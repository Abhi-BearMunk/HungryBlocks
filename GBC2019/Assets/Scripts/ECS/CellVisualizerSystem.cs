//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Entities;

//public class CellVisualizerSystem : ComponentSystem
//{
//    protected override void OnUpdate()
//    {
//        Entities.ForEach((Entity e, Transform transform, CellVisualizer cellVisualizer) =>
//        {
//            transform.position = Vector3.Lerp(transform.position, cellVisualizer.pos, cellVisualizer.moveLerp * Time.deltaTime);
//        });
//    }
//}
