using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootControls : MonoBehaviour, IPausable
{
    public string rHorizontal = "RHorizontal1";
    public string rVertical = "RVertical1";
    public string shoot = "Shoot1";
    public LineRenderer line;

    private Dictionary<Block.CellSubType, Color> typeColors = new Dictionary<Block.CellSubType, Color>();
    private float deadZone = 0.38f;
    private IWeapon weapon;
    private bool shotFired;

    Block block;
    // Start is called before the first frame update
    void Start()
    {
        block = GetComponent<Block>();
        weapon = GetComponent<GrenadeLauncher>();
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        Vector2 direction = new Vector2(Input.GetAxis(rHorizontal), Input.GetAxis(rVertical));
        direction.Normalize();
        Vector2Int directionInt = new Vector2Int(Mathf.Abs(direction.x) > deadZone ? (int)Mathf.Sign(direction.x) : 0, Mathf.Abs(direction.y) > deadZone ? (int)Mathf.Sign(direction.y) : 0);
        if (directionInt.sqrMagnitude > 0)
        {
            // Show aim
            Vector2Int start = block.GetShape().aabbCenter;
            Vector2Int current = start + directionInt;
            CellGroup group = block.GetGrid().GetCellGroup(current);
            while (Continue(group))
            {
                current += directionInt;
                group = block.GetGrid().GetCellGroup(current);
            }
            line.SetPosition(0, (Vector3)((start + new Vector2(0.5f * Mathf.Abs(directionInt.y), 0.5f * Mathf.Abs(directionInt.x))) * block.GetGrid().unitLength) + block.GetGrid().origin - Vector3.forward * 5);
            line.SetPosition(1, (Vector3)((current + new Vector2(0.5f * Mathf.Abs(directionInt.y), 0.5f * Mathf.Abs(directionInt.x))) * block.GetGrid().unitLength) + block.GetGrid().origin - Vector3.forward * 5);

            // Shoot
            if (Input.GetAxis(shoot) > deadZone && !shotFired)
            {
                weapon.Shoot(directionInt);
                shotFired = true;
            }
            else if(Input.GetAxis(shoot) <= deadZone)
            {
                shotFired = false;
            }
        }
        else
        {
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.zero);
        }
    }

    bool Continue(CellGroup group)
    {
        if(group == null)
        {
            return false;
        }
        if(group.cells.Count > 0)
        {
            foreach(Cell cell in group.cells)
            {
                if(cell.GetParentBlock() && cell.GetParentBlock() != block)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void OnBlockSubTypeSet()
    {
        Material instance = new Material(line.materials[0]);
        line.materials[0] = instance;
        typeColors.Add(Block.CellSubType.Default, Color.white);
        typeColors.Add(Block.CellSubType.R, Color.red);
        typeColors.Add(Block.CellSubType.G, Color.green);
        typeColors.Add(Block.CellSubType.B, Color.blue);
        typeColors.Add(Block.CellSubType.Y, Color.yellow);
        line.materials[0].SetColor("_Color", typeColors[block.GetBlockSubType()]);
        line.materials[0].SetColor("_EmissionColor", typeColors[block.GetBlockSubType()]);
    }

    public void SetNewWeapon(IWeapon _weapon)
    {
        weapon = _weapon;
    }
}
