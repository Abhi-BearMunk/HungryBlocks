using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains the visuals for a given Cell
/// </summary>
public class CellVisualizer : MonoBehaviour
{
    public Cell cell;
    public float scaleFactor = 100 / 256;
    public float moveLerp = 30;

    // HACK! This should be somewhere global
    public List<Sprite> baseSprites = new List<Sprite>();
    private Dictionary<Block.CellSubType, Color> typeColors = new Dictionary<Block.CellSubType, Color>();

    // Graphic Objects
    public SpriteRenderer cellBase;
    public GameObject arrow;

    private Vector2 offset;
    private Vector3 pos;
    // Start is called before the first frame update
    void Awake()
    {
        typeColors.Add(Block.CellSubType.Default, Color.white);
        typeColors.Add(Block.CellSubType.R, Color.red);
        typeColors.Add(Block.CellSubType.G, Color.green);
        typeColors.Add(Block.CellSubType.B, Color.blue);
        typeColors.Add(Block.CellSubType.Y, new Color(1,0.75f,0.016f,1));

        offset = new Vector2(0.5f, 0.5f);

        // TODO: Currently all cells draw from a pool of sprites
        cellBase.sprite = baseSprites[Random.Range(0, baseSprites.Count - 1)];
    }

    public void SetScale()
    {
        transform.localScale = scaleFactor * cell.GetParentBlock().GetGrid().unitLength * new Vector3(1, 1, 1);
    }

    public void SetCellSubType()
    {
        cellBase.color = typeColors[cell.GetParentBlock().GetBlockSubType()];
    }

    public void SetPosition()
    {
        //transform.position = (Vector3)(cell.GetGridPosition() + offset) * cell.GetParentBlock().GetGrid().unitLength + cell.GetParentBlock().GetGrid().origin;
        pos = (Vector3)(cell.GetGridPosition() + offset) * cell.GetParentBlock().GetGrid().unitLength + cell.GetParentBlock().GetGrid().origin;
        //StopCoroutine(MoveLerp());
        //StartCoroutine(MoveLerp());
    }

    public void SetPositionImmidiate()
    {
        //transform.position = (Vector3)(cell.GetGridPosition() + offset) * cell.GetParentBlock().GetGrid().unitLength + cell.GetParentBlock().GetGrid().origin;
        transform.position = pos = (Vector3)(cell.GetGridPosition() + offset) * cell.GetParentBlock().GetGrid().unitLength + cell.GetParentBlock().GetGrid().origin;
        //StopCoroutine(MoveLerp());
        //StartCoroutine(MoveLerp());
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, pos, moveLerp * Time.deltaTime);
    }

    IEnumerator MoveLerp()
    {
        while (Vector3.Distance(transform.position, pos) >= 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, pos, moveLerp * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public void Kill()
    {
        //cellBase.color = cellBase.color * new Color(0.5f, 0.5f, 0.5f, 0.1f);
        StartCoroutine(KillSequence());
    }

    IEnumerator KillSequence()
    {
        float scale = 1;
        Vector3 direction = (transform.position - pos).normalized;
        //pos = transform.position + 0.5f * (pos - transform.position);
        moveLerp /= 3;
        while(scale > 0.01f)
        {
            scale -= 0.8f * Time.deltaTime;
            cellBase.transform.localScale = new Vector3(1, 1, 1) * scale;
            cellBase.transform.Rotate(0, 0, 360 * Time.deltaTime);
            //pos = transform.position + direction;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
