using GamepadMapper.Menus;

namespace GamepadMapper.Actuators
{
    public class MenuPointerActuator : IRadialActuator
    {
        public MenuPointerActuator(IMenuController menuController)
        {
            MenuController = menuController;
        }

        public IMenuController MenuController { get; }

        public void Update(double angle, double distance)
        {
            MenuController.UpdatePointer(distance > 0d, angle);
        }
    }
}
