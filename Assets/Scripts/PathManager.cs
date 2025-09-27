using UnityEngine;

public class PathManager : MonoBehaviour
{
    [Header("Настройки пути")]
    public Transform[] pathPoints;    // Массив точек пути
    public Color pathGizmoColor = Color.yellow;

    void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Length < 2) return;

        // Рисуем линии между точками пути
        Gizmos.color = pathGizmoColor;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            if (pathPoints[i] != null && pathPoints[i + 1] != null)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);

                // Рисуем сферы в точках пути
                Gizmos.DrawSphere(pathPoints[i].position, 0.2f);
            }
        }

        // Рисуем последнюю точку
        if (pathPoints[pathPoints.Length - 1] != null)
        {
            Gizmos.DrawSphere(pathPoints[pathPoints.Length - 1].position, 0.2f);
        }
    }

    // Метод для получения точек пути (используется врагами)
    public Vector3[] GetPathPoints()
    {
        Vector3[] points = new Vector3[pathPoints.Length];
        for (int i = 0; i < pathPoints.Length; i++)
        {
            points[i] = pathPoints[i].position;
        }
        return points;
    }
}