using SFML.System;
using SFML.Graphics;
using static bouncyballs.util;

public class circlebody : body {
    private float radius;
    public float Radius {
        get { return radius; }
        set {
            radius = value;
            ((CircleShape)Shape).Radius = value;
            shapeOffset = new Vector2f(-radius, -radius);
        }
    }

    public circlebody(float radius, Vector2f pos, Color colour, float mass = 100) {
        this.shape = new CircleShape();
        this.Radius = radius;
        this.FillColour = colour;
        this.Position = pos;
        this.Mass = mass;
    }
}