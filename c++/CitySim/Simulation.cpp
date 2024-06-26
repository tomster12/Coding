#include "stdafx.h"
#include "Simulation.h"

const float Simulation::QUAD_GAP = 5.0f;
const float Simulation::QUAD_SIZE = 2.0f;
const sf::Color Simulation::GRASS_COL = sf::Color(110, 140, 110);
const float Simulation::ROAD_HWIDTH = 10.0f;
const float Simulation::PATH_HWIDTH = 10.0f;
const float Simulation::NODE_CURVE_SIZE = 17.0f;

Simulation::Simulation(Game* game, sf::RenderWindow* window)
	: game(game), window(window)
{
	game->setClearColor(GRASS_COL);

	// Intialize road network
	roadNetwork = new RoadNetwork();
	roadRenderer = new RoadRenderer(roadNetwork);
	int nodeA = roadNetwork->addNode(500, 320);
	int nodeB = roadNetwork->addNode(250, 250);
	int nodeC = roadNetwork->addNode(400, 600);
	int nodeD = roadNetwork->addNode(800, 220);
	int nodeE = roadNetwork->addNode(830, 450);
	roadNetwork->addSegment(nodeA, nodeB);
	roadNetwork->addSegment(nodeA, nodeC);
	roadNetwork->addSegment(nodeA, nodeD);
	roadNetwork->addSegment(nodeA, nodeE);
	roadNetwork->addSegment(nodeD, nodeE);

	// Initialize building manager
	buildingManager = new BuildingManager(this, roadRenderer);

	// Initialize quads
	/*int countX = 2000;
	int countY = 100;
	size_t count = countX * countY;
	quads = QuadArray(count, QUAD_SIZE);
	for (size_t i = 0; i < count; ++i)
	{
		quads.setPosition(i,
			QUAD_GAP / 2.0f + (int)(static_cast<float>(i) / countY) * (QUAD_SIZE + QUAD_GAP),
			QUAD_GAP / 2.0f + (i % countY) * (QUAD_SIZE + QUAD_GAP)
		);
	}*/
}

Simulation::~Simulation()
{
	delete roadNetwork;
	delete roadRenderer;
}

void Simulation::update()
{
	// Randomly move quads
	for (size_t i = 0; i < quads.getCount(); ++i)
	{
		const sf::Vector2f& pos = quads.getPosition(i);
		quads.setPosition(i,
			pos.x + ((float)rand() / RAND_MAX) * 2.0f - 1.0f,
			pos.y + ((float)rand() / RAND_MAX) * 2.0f - 1.0f
		);
	}
}

void Simulation::queueRenders(DrawQueue& drawQueue)
{
	roadRenderer->queueRenders(drawQueue);
	buildingManager->queueRenders(drawQueue);
	quads.render(window);
}
