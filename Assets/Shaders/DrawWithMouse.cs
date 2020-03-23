using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWithMouse : MonoBehaviour
{
    [SerializeField] Camera _Camera;
    [SerializeField] Shader _drawShader;
    [SerializeField] GameObject _terrain;
    [SerializeField] BoxCollider PlayerCollider;
    // Start is called before the first frame update
    private RenderTexture _splatMap;
    private Material _snowMaterial, _drawMaterial;
    private RaycastHit _hit;
    public Transform[] _foot;
    int _layerMask;
    [Range(0.01f, 500)]
    [SerializeField] float _brushSize;
    [Range(0, 1)]
    [SerializeField] float _brushStrength;
    void Start()
    {
        _layerMask = LayerMask.GetMask("Ground");
        _drawMaterial = new Material(_drawShader);
        _drawMaterial.SetVector("_Color", Color.red);
        _snowMaterial = _terrain.GetComponent<MeshRenderer>().material;
        _splatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        _snowMaterial.SetTexture("_SplatTex", _splatMap);

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _foot.Length; i++)
        {
            Debug.DrawRay(new Vector3(_foot[i].position.x, _foot[i].position.y + 0.5f, _foot[i].position.z), -Vector3.up, Color.yellow);
            if (Physics.Raycast(new Vector3(_foot[i].position.x, PlayerCollider.bounds.min.y + 0.5f, _foot[i].position.z), -Vector3.up, out _hit, 1f, _layerMask))
            {
                Debug.DrawRay(new Vector3(_foot[i].position.x, PlayerCollider.bounds.min.y + 0.5f, _foot[i].position.z), -Vector3.up, Color.red);
                _drawMaterial.SetVector("_Coordinate", new Vector4(_hit.textureCoord.x, _hit.textureCoord.y, 0, 0));
                _drawMaterial.SetFloat("_Strength", _brushStrength);
                _drawMaterial.SetFloat("_Size", _brushSize);
                RenderTexture temp = RenderTexture.GetTemporary(_splatMap.width, _splatMap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(_splatMap, temp);
                Graphics.Blit(temp, _splatMap, _drawMaterial);
                RenderTexture.ReleaseTemporary(temp);

            }
        }
    }
    private void OnGUI()
    {
        //GUI.DrawTexture(new Rect(0, 0, 128, 128), _splatMap, ScaleMode.ScaleToFit, false, 1);
    }
}
