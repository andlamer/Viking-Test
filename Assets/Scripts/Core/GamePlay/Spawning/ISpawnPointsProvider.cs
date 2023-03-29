using UnityEngine;

namespace VikingTest.Core.Gameplay
{
    public interface ISpawnPointsProvider
    {
        Vector3[] GetStartingPositions(int numberOfPoints);
        Vector3 GetRandomPositionOnField();
    }
}