using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TextureList
{
    public List<Sprite> List;
}


public class SeasonChanger : MonoBehaviour
{
    public List<GameObject> GridList = new List<GameObject>();

    public List<GameObject> SpriteList = new List<GameObject>();

    public List<TextureList> TextureList = new List<TextureList>();

    public GameObject Trash;

    public List<Material> TrashShaders = new List<Material>();

    public List<GameObject> ToxicLights = new List<GameObject>();

    public int ActiveTilePalette;

    public List<GameObject> TilePalettesList = new List<GameObject>();

    public Material FoxMaterial;


    // Start is called before the first frame update
    void Start()
    {
        FoxMaterial.color = new Color(255 / 255f, 247 / 255f, 210 / 255f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeSeason(int season)
    {

        if (season == ActiveTilePalette) return;


        Tilemap currentTilePaletteTilemap = TilePalettesList[ActiveTilePalette].GetComponentInChildren<Tilemap>();
        Tilemap newTilePaletteTilemap = TilePalettesList[season].GetComponentInChildren<Tilemap>();

        var d = new Dictionary<TileBase, TileBase>();
        var n  = new BoundsInt(newTilePaletteTilemap.origin, newTilePaletteTilemap.size).allPositionsWithin;
        n.Reset();

        foreach (var c in new BoundsInt(currentTilePaletteTilemap.origin, currentTilePaletteTilemap.size).allPositionsWithin)
        {
            n.MoveNext();
            var currentTile = currentTilePaletteTilemap.GetTile(c);
            var newTile = newTilePaletteTilemap.GetTile(n.Current);
            if (currentTile != null && newTile != null)
            {
                d[currentTile] = newTile;
            }
        }

        foreach (GameObject grid in GridList)
        {
            foreach (var pair in d)
            {
                if (pair.Key != pair.Value)
                grid.GetComponent<Tilemap>().SwapTile(pair.Key, pair.Value);
            }
        }


        for (int i = 0; i < SpriteList.Count; i++)
        {
            SpriteList[i].GetComponent<SpriteRenderer>().sprite = TextureList[i].List[season];
        }

        if (Trash != null)
        {
            Trash.GetComponent<TilemapRenderer>().material = TrashShaders[season];
        }

        for (int i = 0; i < ToxicLights.Count && season == 2; i++)
        {
            ToxicLights[i].GetComponent<Light2D>().color = new Color(1, 0, 0);
        }

        if (season == 0)
            FoxMaterial.color = new Color(255 / 255f, 247 / 255f, 210 / 255f);
        if (season == 1)
            FoxMaterial.color = new Color(255 / 255f, 181 / 255f, 90 / 255f);
        if (season == 2)
            FoxMaterial.color = new Color(202 / 255f, 94 / 255f, 85 / 255f);

        ActiveTilePalette = season;
    }
}