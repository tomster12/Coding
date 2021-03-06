
#pragma once


class UIElement {

public:
	virtual void update() {};
	virtual void render(sf::RenderWindow* window) {};
};


class UIButton : public UIElement {

private:
	sf::RenderWindow* window;
	sf::Vector2f pos;
	sf::Vector2f size;
	sf::Texture texture;
	sf::Sprite sprite;
	std::function<void()> action;

	bool isHovered;
	bool isPressed;


public:
	static const sf::Color HOVER_COLOR;

	UIButton(sf::RenderWindow* window, sf::Vector2f pos, sf::Vector2f size, std::string texturePath, std::function<void()> action);

	void update() override;
	void render(sf::RenderWindow* window) override;
};


class UIToggleButton : public UIElement {

private:
	sf::RenderWindow* window;
	sf::Vector2f pos;
	sf::Vector2f size;
	sf::Texture texture;
	sf::Sprite sprite;
	std::function<void(bool)> action;

	bool isToggled;
	bool isHovered;
	bool isPressed;


public:
	static const sf::Color TOGGLE_COLOR;

	UIToggleButton(sf::RenderWindow* window, sf::Vector2f pos, sf::Vector2f size, std::string texturePath, bool initial, std::function<void(bool)> action);

	void update() override;
	void render(sf::RenderWindow* window) override;
};


class UIManager {

private:
	std::vector<UIElement*> uiElements;


public:
	~UIManager();

	void update();
	void render(sf::RenderWindow* window);

	void addElement(UIElement* uiElement);
};
