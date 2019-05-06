//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Entities;

//public class BlockMoverSystem : ComponentSystem
//{
//    protected override void OnUpdate()
//    {
//        Entities.ForEach((Entity e, Transform transform, Block block, BlockMover mover) =>
//        {
//            mover.deltaPosition += mover.velocity * Time.deltaTime;
//            if (mover.deltaPosition.x >= 1)
//            {
//                block.Translate(1, 0);
//                mover.deltaPosition.x = 0;
//            }
//            else if (mover.deltaPosition.x <= -1)
//            {
//                block.Translate(-1, 0);
//                mover.deltaPosition.x = 0;
//            }
//            if (mover.deltaPosition.y >= 1)
//            {
//                block.Translate(0, 1);
//                mover.deltaPosition.y = 0;
//            }
//            else if (mover.deltaPosition.y <= -1)
//            {
//                block.Translate(0, -1);
//                mover.deltaPosition.y = 0;
//            }
//        });
//    }
//}
