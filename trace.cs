using SFML.System;
using SFML.Graphics;
using static bouncyballs.util;

public class trace {
    private Vector2f origin;
    public Vector2f Origin {
        get { return origin; }
        set { origin = value; }
    }

    private Vector2f direction;
    public Vector2f Direction {
        get { return direction; }
        set { direction = value; }
    }

    private float resolution;
    public float Resolution {
        get { return resolution; }
        set { resolution = value; }
    }

    private float distance;
    public float Distance {
        get { return distance; }
        set { distance = value; }
    }

    private bool hit = false;
    public bool Hit {
        get { return hit; }
    }

    private Vector2f? hitPosition;
    public Vector2f? HitPosition {
        get { return hitPosition; }
    }

    public trace(Vector2f origin, Vector2f direction, float distance = 100f, float resolution = 10f) {
        this.Origin = origin;
        this.Direction = direction;
        this.Distance = distance;
        this.Resolution = resolution;
    }

    public void fire(List<body> bodies) {
        int points = (int)Math.Ceiling(this.Distance / this.Resolution);
        Vector2f increment = this.Direction * Distance / Resolution;
        Vector2f point = origin + increment;
        for (int i = 1; i <= points; i++) {
            if (this.hit) { break; }

            // check if the point is within the body
            foreach(body b in bodies) {
                if (b.GetType() == typeof(rectbody)) {
                    rectbody rb = (rectbody)b;

                    if ((point.X >= rb.Position.X ||
                         point.X <= rb.Position.X + rb.Size.X) &&
                        (point.Y >= rb.Position.Y ||
                         point.Y <= rb.Position.Y + rb.Size.Y)) {
                        this.hit = true;
                        this.hitPosition = point;
                        break;
                    }
                } else if (b.GetType() == typeof(circlebody)) {
                    circlebody cb = (circlebody)b;

                    if (distance(point, cb.Position) <= cb.Radius) {
                        this.hit = true;
                        this.hitPosition = point;
                        break;
                    }
                }
            }

            point += increment;
        }
    }

    public static trace doTrace(List<body> bodies, Vector2f origin, Vector2f direction, float resolution = 10f) {
        trace T = new trace(origin, direction, resolution);
        T.fire(bodies);
        return T;
    }
}