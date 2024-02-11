#pragma once

#include "DrawQueue.h"
#include "RoadNetwork.h"
#include "RoadRenderer.h"

class Game;
class Simulation;
class World;

class PlayerController
{
public:
	PlayerController(Game* game, Simulation* simulation, World* world);
	~PlayerController();
	void update();
	void queueRenders(DrawQueue& drawQueue);

private:
	Game* game;
	Simulation* simulation;
	World* world;

	sf::RenderWindow const* window;
	sf::View camView;
	sf::Vector2f baseViewSize;
	sf::Vector2f camPos;
	float camZoom;
	float camZoomVel;

	bool nodePlacementLock = false;
	RoadNetwork* buildingRoadNetwork;
	RoadRenderer* buildingRoadRenderer;
	int buildingNodeFrom, buildingBNodeFrom, buildingBNodeTo;

	sf::CircleShape indicator;

	static const float CAM_POS_VEL;
	static const float CAM_SCROLL_ACC;
	static const float CAM_SCROLL_DRAG;
};
