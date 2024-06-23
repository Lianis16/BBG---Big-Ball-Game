
namespace BoalsProject
{
    internal static class GlobalActions
    {
        public static Action UpdateAction;

        public static void Update()
        {
            if (UpdateAction != null) UpdateAction.Invoke();
        }
    }
}
