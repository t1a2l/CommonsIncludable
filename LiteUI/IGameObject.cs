namespace Commons.LiteUI
{
    public interface IGameObject
    {
        void Update();
    }

    public interface IDestroyableObject
    {
        void OnDestroy();
    }

    public interface IAwakingObject
    {
        void Awake();
    }

    public interface IUIObject
    {
        void OnGUI();
    }
}
