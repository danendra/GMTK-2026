using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class ParallaxController : MonoBehaviour
{
    [Tooltip("Kecepatan scroll dalam satuan UV per detik. Makin besar makin cepat.")]
    [SerializeField] private float scrollSpeed = 0.5f;

    [Tooltip("Arah scroll di ruang UV. (0,1) menghasilkan gerak KE BAWAH di layar. " +
             "(0,-1) ke atas, (1,0) ke kiri, (-1,0) ke kanan.")]
    [SerializeField] private Vector2 scrollDirection = Vector2.up;

    private Material mat;
    private Vector2 offset;

    private void Start()
    {
        // .material meng-instance material, jadi asset aslinya tidak ikut berubah.
        mat = GetComponent<SpriteRenderer>().material;
        offset = mat.mainTextureOffset;
    }

    private void Update()
    {
        // Naikkan offset. Di ruang UV, menaikkan offset.y bikin texture terlihat
        // bergerak ke bawah di layar (V naik ke atas, jadi sampling-nya "geser turun").
        offset += scrollDirection.normalized * scrollSpeed * Time.deltaTime;

        // Bungkus ke [0,1) supaya presisi float tetap bagus walau game jalan lama.
        offset.x = Mathf.Repeat(offset.x, 1f);
        offset.y = Mathf.Repeat(offset.y, 1f);

        mat.mainTextureOffset = offset;
    }

    private void OnDestroy()
    {
        // .material membuat instance baru; hancurkan biar tidak bocor memori.
        if (mat != null)
            Destroy(mat);
    }
}