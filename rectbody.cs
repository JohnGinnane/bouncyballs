using SFML.System;
using SFML.Graphics;
using static bouncyballs.util;

public class rectbody : body {
    private Vector2f size;
    public Vector2f Size {
        get {return size;}
        set { 
            size = value;
            ((RectangleShape)Shape).Size = value;
            shapeOffset = new Vector2f(-size.X / 2f, -size.Y / 2f);
        }
    }
    public rectbody(Vector2f size, Vector2f pos, Color colour, float mass = 100) {
        this.shape = new RectangleShape();
        this.Size = size;
        this.FillColour = colour;
        this.Position = pos;
        this.Mass = mass;
    }
}