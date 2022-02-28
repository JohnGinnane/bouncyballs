using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static bouncyballs.util;

namespace bouncyballs {
    static class Program {
        static void Main(string[] args) {
            bouncyballs bb = new bouncyballs();
            bb.run();
        }
    }

    public class bouncyballs {     
#region "Properties"
        Vector2f viewPos;
        Vector2f screenSize;

        float delta = 0;
        double RunTime = 0; // time program has been running
        DateTime lastTime = DateTime.Now;
        float timeScale = 1f;

        float gravityMulti = 9.81f;

        Random rand = new Random();
        Vector2i mousePos;
        bool grabbedWindow = false;
        Vector2i grabbedOffset;
        Vector2i lastMousePos;
        
        Font arial;
        RenderWindow window;
        View view;

        List<body>bodies = new List<body>();

        float timestep = 1f / 100f;
        float maxSpeed = 1000f;
#endregion

#region "Methods"
        public bouncyballs() {
            arial = new Font("arial.ttf");
            screenSize = new Vector2f(800, 600);
            window = new RenderWindow(new VideoMode((uint)screenSize.X, (uint)screenSize.Y),
                                      "Bouncy Balls!",
                                      Styles.Close | Styles.Titlebar);

            window.Closed += window_CloseWindow;
            window.Resized += window_ResizedWindow;
            window.KeyPressed += window_KeyPressed;
            window.KeyReleased += window_KeyReleased;
            window.MouseButtonPressed += window_MouseButtonPressed;
            window.MouseButtonReleased += window_MouseButtonReleased;
            window.MouseMoved += window_MouseMoved;

            viewPos = screenSize / 2;

            view = new View(viewPos, screenSize);
            window.SetView(view);
            window.SetKeyRepeatEnabled(false);
        }

        public void run() {
            Color grey = new Color(100, 100, 100);
            float borderThickness = 10f;

            // edge of window is just a fixed rectangle
            rectbody topEdge = new rectbody(new Vector2f(screenSize.X, borderThickness),
                                            new Vector2f(screenSize.X / 2f, borderThickness / 2f),
                                            grey);
            topEdge.isStatic = true;
            bodies.Add(topEdge);

            rectbody bottomEdge = new rectbody(new Vector2f(screenSize.X, borderThickness),
                                               new Vector2f(screenSize.X / 2f, screenSize.Y - borderThickness / 2f),
                                               grey);
            bottomEdge.isStatic = true;
            bodies.Add(bottomEdge);

            rectbody leftEdge = new rectbody(new Vector2f(borderThickness, screenSize.Y),
                                             new Vector2f(borderThickness / 2f, screenSize.Y / 2f),
                                             grey);
            leftEdge.isStatic = true;
            bodies.Add(leftEdge);

            rectbody rightEdge = new rectbody(new Vector2f(borderThickness, screenSize.Y),
                                              new Vector2f(screenSize.X - borderThickness / 2f, screenSize.Y / 2f),
                                              grey);
            rightEdge.isStatic = true;
            bodies.Add(rightEdge);

            int numBalls = 10;
            for(int i = 0; i < numBalls; i++) {
                float radius = 10f + (float)rand.NextDouble() * 10f;
                float mass = randfloat(100, 300);
                Vector2f pos = randvec2(radius, screenSize.X-radius, radius, screenSize.Y-radius);
                //ball B = new ball(radius, pos, hsvtocol(360f/balls.Length * i, 1, 1));
                circlebody B = new circlebody(radius, pos, hsvtocol(360f/numBalls * i, 1, 1), mass);
                B.Velocity = randvec2(-50, 50);
                B.Bounciness = 0.95f; // bouncy balls are 90% efficient in collisions
                bodies.Add(B);
            }
            
            // bucket hit box
            rectbody bucketBottom = new rectbody(new Vector2f(100, 20),
                                                 screenSize / 2f,
                                                 grey);
            bucketBottom.isStatic = true;
            bodies.Add(bucketBottom);

            rectbody bucketLeft = new rectbody(new Vector2f(20, 100),
                                               bucketBottom.Position + new Vector2f(-40, -60),
                                               grey);
            bucketLeft.isStatic = true;
            bodies.Add(bucketLeft);

            rectbody bucketRight = new rectbody(new Vector2f(20, 100),
                                                bucketBottom.Position + new Vector2f(40, -60),
                                                grey);
            bucketRight.isStatic = true;
            bodies.Add(bucketRight);

            // main loop
            while (window.IsOpen) {
                if ((float)(DateTime.Now - lastTime).TotalSeconds < timestep) {
                    continue;
                }

                mousePos = Mouse.GetPosition();
                //delta = (float)(DateTime.Now - lastTime).TotalMilliseconds / 1000;
                delta = timestep;
                lastTime = DateTime.Now;
                RunTime += delta;
                
                window.Clear();

                // get window velocity if its being moved
                Vector2f windowVel = new Vector2f();

                if (grabbedWindow) {
                    float mouseMoveMulti = 300f;
                    windowVel += (Vector2f)(Mouse.GetPosition() - lastMousePos) * delta * mouseMoveMulti * timeScale;
                }
                
                lastMousePos = Mouse.GetPosition();

                foreach(body b in bodies) {
                    for (int i = 0; i < bodies.Count; i++) {                        
                        body c = bodies[i];
                        if (b == c) { continue; }

                        // TRYING TO PREVENT TUNNELLING START
                        // check for time of impact to try 
                        // alleviate tunneling
                        // Vector2f cbdir = normalise(c.Position - b.Position);
                        // Vector2f veldir = normalise(b.Velocity);

                        // Vector2f nextPos = b.Position + b.Velocity * delta * timeScale;

                        // // if the body is on the opposite side of the other
                        // // body then we have gone too far!
                        // Vector2f curPosToOtherBody = normalise(c.Position - b.Position);
                        // Vector2f nextPosToOtherBody = normalise(c.Position - nextPos);

                        // if (dot(curPosToOtherBody, nextPosToOtherBody) < 0) {
                        //     string reportIssue = "Tunnelling has potentially occurred!";
                        //     // if the body is moving towards the other body
                        //     // then we will check for potential tunnelling
                        //     Vector2f bcnorm = normalise(c.Position - b.Position);
                        //     Vector2f velnorm = normalise(b.Velocity * delta * timeScale);
                        //     float normdotnorm = dot(bcnorm, velnorm);
                        //     if (normdotnorm > 0) {
                        //         float dist = distance(c.Position, b.Position);
                        //         float velmag = magnitude(b.Velocity * delta * timeScale);

                        //         float timeOfImpact = dist / velmag / normdotnorm;

                        //         if (timeOfImpact < 1) {
                        //             reportIssue += " (2)";
                        //             b.SetVelocity(new Vector2f());
                        //             b.isStatic = true;
                        //             b.OutlineColour = Color.White;
                        //         }
                        //     }

                        //     Console.WriteLine(reportIssue);
                        // }
                        // TRYING TO PREVENT TUNNELLING END
                        
                        b.collide(c);
                    }

                    if (b.GetType() == typeof(circlebody)) {
                        circlebody cb = (circlebody)b;
                        cb.SetVelocity(cb.Velocity + windowVel);

                        //gravity
                        cb.SetYVelocity(cb.Velocity.Y + cb.Mass * gravityMulti * delta * timeScale);
                    }

                    if (magnitude(b.Velocity) > maxSpeed) {
                        b.SetVelocity(normalise(b.Velocity) * maxSpeed);
                    }

                    //b.SetVelocity(new Vector2f(maxSpeed / b.Velocity.X, maxSpeed / b.Velocity.Y));
                    b.update(delta  * timeScale);
                    b.draw(window);
                }

                window.DispatchEvents();
                window.Display();
            } // end while(window.IsOpen)
        }
#endregion
        
#region "Events"
        
        void window_CloseWindow(object? sender, System.EventArgs? e) {
            if (sender == null) { return; }
            ((RenderWindow)sender).Close();
        }

        void window_ResizedWindow(object? sender, System.EventArgs? e) {
            if (sender == null) { return; }
            if (e == null) { return; }

            SizeEventArgs sizeE = (SizeEventArgs)e;
            screenSize = new Vector2f(sizeE.Width, sizeE.Height);
            view.Reset(new FloatRect(0, 0, sizeE.Width, sizeE.Height));
            window.SetView(view);

            //Console.WriteLine("Resized window!"); 
        }

        void window_MouseButtonPressed(object? sender, System.EventArgs e) {
            if (sender == null) { return; }
            if (e == null) { return; }

            if (e.GetType() == typeof(MouseButtonEventArgs)) {
                MouseButtonEventArgs mouseButton = (MouseButtonEventArgs)e;
                //Console.WriteLine(String.Format("Button {0} pressed at ({1}, {2})", mouseButton.Button.ToString(), mouseButton.X.ToString(), mouseButton.Y.ToString()));

                if (mouseButton.Button == Mouse.Button.Left) {
                    grabbedOffset = (Vector2i)window.Position - Mouse.GetPosition();
                    grabbedWindow = true;
                }
            }
        }

        void window_MouseButtonReleased(object? sender, System.EventArgs e) {
            if (sender == null) { return; }
            if (e == null) { return; }

            if (e.GetType() == typeof(MouseButtonEventArgs)) {
                MouseButtonEventArgs mouseButton = (MouseButtonEventArgs)e;
                //Console.WriteLine(String.Format("Button {0} released at ({1}, {2})", mouseButton.Button.ToString(), mouseButton.X.ToString(), mouseButton.Y.ToString()));

                if (mouseButton.Button == Mouse.Button.Left) {
                    grabbedWindow = false;
                }
            }
        }

        void window_MouseMoved(object? sender, System.EventArgs e) {
            if (sender == null) { return; }
            if (e == null) { return; }

            if (e.GetType() == typeof(MouseMoveEventArgs)) {
                if (grabbedWindow) {
                    window.Position = Mouse.GetPosition() + grabbedOffset;
                }
            }
        }

        void window_KeyPressed(object? sender, System.EventArgs e) {
            if (sender == null) { return; }
            if (e == null) { return; }

            if (e.GetType() == typeof(KeyEventArgs)) {
                KeyEventArgs key = (KeyEventArgs)e;
                //Console.WriteLine(String.Format("Key {0} was pressed", key.Code.ToString()));

                if (key.Code == Keyboard.Key.Escape) {
                    window.Close();
                }

                if (key.Alt && key.Code == Keyboard.Key.F4) {
                    window.Close();
                }
            }
        }
        
        void window_KeyReleased(object? sender, System.EventArgs e) {
            if (sender == null) { return; }
            if (e == null) { return; }

            if (e.GetType() == typeof(KeyEventArgs)) {
                KeyEventArgs key = (KeyEventArgs)e;
                //Console.WriteLine(String.Format("Key {0} was released", key.Code.ToString()));
            }
        }
#endregion
    }
}