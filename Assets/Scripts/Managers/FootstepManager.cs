using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FootstepManager : MonoBehaviour {
    public AudioSource footstepSource;

    public List<AudioClip> grassSteps = new List<AudioClip>();
    public List<AudioClip> dirtSteps = new List<AudioClip>();
    private List<AudioClip> currentList;

    // Since we only have one map, and the ground player can actually step
    // on only has 2 types: dirt and grass. So I only take the middle tilemap which contains grass blocks
    // And if there is a tile, it should be grass, otherwise, it should be dirt.
    // TODO: If we need to switch between different scenes/maps,
    // TODO: then there should be sth like `MapManager` that contains
    // TODO: curTilemap to replace this field?
    private Tilemap middleTilemap;
    private GameObject player;

    // The BlendTree in the animator will cause multiple animation plays simutanously,
    // so we need to debounce the footstep event.
    public float debounceTime = 0.3f;
    private float timer = 0.0f;
    private bool isDebouncing;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindWithTag("Player");
        Tilemap tilemap = GameObject.FindWithTag("FootstepTilemap").GetComponent<Tilemap>();
        if (tilemap != null) {
            middleTilemap = tilemap;
        }
    }

    // Update is called once per frame
    void Update() {
        // In case the scene is loaded after the Start() method?
        if (middleTilemap == null) {
            Tilemap tilemap = GameObject.Find("Ground Middle").GetComponent<Tilemap>();
            if (tilemap != null) {
                middleTilemap = tilemap;
            }
        }

        Vector3Int cellPos = middleTilemap.WorldToCell(player.transform.position);
        TileBase tile = middleTilemap.GetTile(cellPos);
        // See the comments on the `middleTilemap` filed
        if (tile == null) {
            currentList = dirtSteps;
        }
        else {
            currentList = grassSteps;
        }

        if (isDebouncing) {
            timer += Time.deltaTime;

            if (timer >= debounceTime) {
                isDebouncing = false;
            }
        }
    }

    public void PlayFootStep() {
        if (currentList == null || isDebouncing)
            return;

        isDebouncing = true;
        AudioClip clip = currentList[Random.Range(0, currentList.Count)];
        footstepSource.PlayOneShot(clip);
    }
}
