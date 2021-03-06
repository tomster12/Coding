
#pragma once

#include "UtilityFunctions.h"


namespace tbml {

	template<class D> // D: GeneticData<D>
	class GeneticData {

	public:
		virtual void randomize() = 0;

		virtual void mutate(float chance) = 0;

		virtual D* crossover(D* data) = 0;
	};


	template<class D> // D: GeneticData<D>
	class GeneticInstance {

	protected:
		D* geneticData;
		bool instanceFinished;
		float instanceFitness;


	public:
		GeneticInstance(D* geneticData)
			: geneticData(geneticData), instanceFinished(false), instanceFitness(0.0f) {};


		virtual void step() = 0;

		virtual void render(sf::RenderWindow* window) = 0;


		virtual bool getInstanceFinished() = 0;

		virtual float getInstanceFitness() = 0;
	};


	template<class D, class I> // D: GeneticData<D>, I: GeneticInstance<D>
	class GenepoolSimulation {

	protected:
		struct GeneticEvaluation {
			D* data;
			I* instance;
			bool isFinished;
			float fitness;
		};


		int generationCount;
		float mutationRate;

		bool isInitialized = false;
		std::vector<GeneticEvaluation> currentGeneration;
		int generationNumber;
		int generationStep;
		bool isRunning;
		bool isFinished;
		bool autoStart;
		bool autoFinish;
		bool autoComplete;


		virtual D* createData() {
			// Create, randomize and return data
			D* data = new D();
			data->randomize();
			return data;
		}

		virtual I* createInstance(D* data) {
			// Create and return instance
			I* inst = new I(data);
			return inst;
		}


	public:
		~GenepoolSimulation() {
			if (!this->isInitialized) return;

			// Delete all data / instances
			for (int i = 0; i < this->generationCount; i++) {
				delete this->currentGeneration[i].data;
				delete this->currentGeneration[i].instance;
			}
		}

		void initGenepool(int generationCount, float mutationRate) {
			// Delete previous
			if (this->isInitialized) {
				// Delete all data / instances
				for (int i = 0; i < this->generationCount; i++) {
					delete this->currentGeneration[i].data;
					delete this->currentGeneration[i].instance;
				}
			}

			// Initialize variables
			this->generationCount = generationCount;
			this->mutationRate = mutationRate;

			this->isInitialized = true;
			this->currentGeneration = std::vector<GeneticEvaluation>();
			this->generationNumber = 1;
			this->generationStep = 0;
			this->isRunning = false;
			this->isFinished = false;
			this->autoStart = false;
			this->autoFinish = false;
			this->autoComplete = false;

			// [INITIALIZATION] Initialize all data and instances
			for (int i = 0; i < this->generationCount; i++) {
				D* data = createData();
				I* inst = createInstance(data);
				this->currentGeneration.push_back({ data, inst, false, 0.0f });
			}
			initGeneration();
		};


		void update() {
			if (!this->isInitialized) return;
			if (this->isFinished || !this->isRunning) return;

			// Step / complete generation then check for auto finish
			if (this->autoComplete) fullStepGeneration();
			else singleStepGeneration();
			if (this->autoFinish && this->isFinished) finishGeneration();
		};

		virtual void render(sf::RenderWindow* window) {
			if (!this->isInitialized) return;

			// Render all current generation
			for (auto& eval : currentGeneration) eval.instance->render(window);
		};


		virtual void initGeneration() {}

		void startGeneration() {
			// Start all current generation
			if (!this->isInitialized) return;
			if (this->isRunning || this->isFinished) return;
			this->isRunning = true;
		};

		void pauseGeneration() {
			// Start all current generation
			if (!this->isInitialized) return;
			if (!this->isRunning || this->isFinished) return;
			this->isRunning = false;
		};

		void singleStepGeneration() {
			if (!this->isInitialized) return;
			if (this->isFinished) return;

			// Step all unfinished instances
			bool allFinished = true;
			for (GeneticEvaluation& eval : currentGeneration) {
				if (!eval.isFinished) {
					eval.instance->step();

					// Cache final fitness once finished
					if (eval.instance->getInstanceFinished()) {
						eval.isFinished = true;
						eval.fitness = eval.instance->getInstanceFitness();
					} else allFinished = false;
				}
			}

			// Once all finished update variables
			this->generationStep++;
			if (allFinished) {
				this->isFinished = true;
				this->isRunning = false;
			}
		}

		void fullStepGeneration() {
			if (!this->isInitialized) return;

			// Step generation until finished
			while (!this->isFinished) singleStepGeneration();
			if (this->autoFinish) finishGeneration();
		}

		void finishGeneration() {
			if (!this->isInitialized) return;
			if (this->isRunning || !this->isFinished) return;

			// [SELECTION] Sort, then cull the bottom half of the generation
			std::sort(this->currentGeneration.begin(), this->currentGeneration.end(), [this](auto a, auto b) { return a.fitness < b.fitness; });
			int amount = static_cast<int>(this->currentGeneration.size() / 2);
			std::vector<GeneticEvaluation> selectedGeneration(this->currentGeneration.begin() + amount, this->currentGeneration.end());
			std::cout << "Generation: " << this->generationNumber << ", best fitness: " << selectedGeneration[selectedGeneration.size() - 1].fitness << std::endl;

			// setup parent selection function
			float totalFitness = 0.0f;
			for (auto& eval : selectedGeneration) totalFitness += eval.fitness;
			auto pickParent = [selectedGeneration, totalFitness](float r) {
				float cumSum = 0.0f;
				for (auto& eval : selectedGeneration) {
					cumSum += eval.fitness / totalFitness;
					if (r <= cumSum) return eval.data;
				}
				return selectedGeneration[selectedGeneration.size() - 1].data;
			};

			// Create the next generation
			std::vector<GeneticEvaluation> newGeneration;
			for (int i = 0; i < this->generationCount; i++) {
				D* dataA = pickParent(getRandomFloat());
				D* dataB = pickParent(getRandomFloat());

				// [CROSSOVER], [MUTATION] Create new genetic data
				D* newData = dataA->crossover(dataB);
				newData->mutate(this->mutationRate);
				I* newInst = createInstance(newData);
				GeneticEvaluation newEval = { newData, newInst, false, 0.0f };
				newGeneration.push_back(newEval);
			}

			// Delete old, replace with new, update variables
			for (auto& eval : this->currentGeneration) { delete eval.data; delete eval.instance; }
			this->currentGeneration = newGeneration;
			this->generationNumber++;
			this->isFinished = false;
			initGeneration();

			// Handle auto start
			if (this->autoStart) startGeneration();
		};


		bool getAutoStart() { return this->autoStart; };

		bool getAutoFinish() { return this->autoFinish; };

		bool getAutoComplete() { return this->autoComplete; };

		void setAutoStart(bool autoStart) { this->autoStart = autoStart; if (!this->isRunning && this->autoStart) this->startGeneration(); };

		void setAutoFinish(bool autoFinish) { this->autoFinish = autoFinish; if (this->isFinished && this->autoFinish) this->finishGeneration(); }

		void setAutoComplete(bool autoComplete) { this->autoComplete = autoComplete; if (this->autoComplete) this->fullStepGeneration(); }
	};
}
