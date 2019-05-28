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
    // HACK! Write custom inspector in some global class
    public List<Sprite> typeSprites = new List<Sprite>();
    public static Dictionary<Block.CellSubType, Color> typeColors = new Dictionary<Block.CellSubType, Color>();

    // Graphic Objects
    public SpriteRenderer cellBase;
    public SpriteRenderer cellGlow;
    public GameObject arrow;

    private Vector2 offset;
    [HideInInspector]
    public Vector3 pos;

    private bool scrollTexture = false;
    public float scrollSpeed = 1;
    public float glowH = 0;
    int glowDir = -1;
    // Start is called before the first frame update
    void Awake()
    {
        if(typeColors.Count == 0)
        {
            typeColors.Add(Block.CellSubType.Default, Color.white);
            typeColors.Add(Block.CellSubType.R, Color.red);
            typeColors.Add(Block.CellSubType.G, Color.green);
            typeColors.Add(Block.CellSubType.B, Color.blue);
            typeColors.Add(Block.CellSubType.Y, new Color(1, 0.75f, 0.016f, 1));
        }

        offset = new Vector2(0.5f, 0.5f);

        // TODO: Currently all cells draw from a pool of sprites
        //cellBase.sprite = baseSprites[Random.Range(0, baseSprites.Count - 1)];
    }

    public void SetScale()
    {
        transform.localScale = scaleFactor * cell.GetParentBlock().GetGrid().unitLength * new Vector3(1, 1, 1);
    }

    public void SetCellType()
    {
        int type = (int)cell.GetParentBlock().GetBlockType();
        cellBase.sprite = typeSprites[type];
        //if(cell.GetParentBlock().GetBlockType() == Block.CellType.PowerUp)
        //{
        //    cellGlow.enabled = true;
        //}
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
        if (cell.GetParentBlock().GetBlockType() == Block.CellType.PowerUp)
        {

            glowH += glowDir * scrollSpeed * Time.deltaTime;
            if (glowH >= 1)
            {
                glowDir = -1;
                glowH = 1;
            }
            if (glowH <= 0)
            {
                glowDir = 1;
                glowH = 0;
            }
            cellGlow.color = Color.HSVToRGB(glowH, 1, 1);
        }
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
        if(cell.GetParentBlock().GetBlockType() == Block.CellType.PowerUp)
        {
            StartCoroutine(KillSequence2());

        }
        else
        {
            //pos += 0.5f * (Vector3)Random.insideUnitCircle;
            StartCoroutine(KillSequence());
        }
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

    IEnumerator KillSequence2()
    {
        cellGlow.enabled = false;
        float scale = 0.6f;
        cellBase.transform.localScale = new Vector3(1, 1, 1) * scale;
        yield return new WaitForSeconds(0.35f);
        while (scale < 2)
        {
            scale += 1.5f * Time.deltaTime;
            cellBase.transform.localScale = new Vector3(1, 1, 1) * scale;
            Color col = cellBase.color;
            col.a = 2 - scale;
            cellBase.color = col;
            //pos = transform.position + direction;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
