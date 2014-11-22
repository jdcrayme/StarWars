using UnityEngine;

namespace Assets.Game.Interface.Cameras
{
    public interface ICameraControler
    {
        void Update(Camera cam);

        string DataString { get; }
    }
}
