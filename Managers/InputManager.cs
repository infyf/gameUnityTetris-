using UnityEngine;

namespace Managers
{
    public class InputManager
    {
        public int direction;
        public bool rotate;
        public bool fastFall;

        public void HandleInput()
        {
            direction = 0;
            rotate = false;

            if (Input.GetKeyDown(KeyCode.A))
                direction = -1;

            if (Input.GetKeyDown(KeyCode.D))
                direction = 1;

            if (Input.GetKeyDown(KeyCode.W))
                rotate = true;

            fastFall = Input.GetKey(KeyCode.S);
        }
    }
}
