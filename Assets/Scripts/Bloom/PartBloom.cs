using System;
using UnityEngine;
using System.Collections;

public class PartBloom : MonoBehaviour
{
    //采样率
    public int samplerScale = 1;
    //高亮部分提取阈值
    public Color colorThreshold = Color.gray;
    //Bloom泛光颜色
    public Color bloomColor = Color.white;
    //Bloom权值
    [Range(0.0f, 1.0f)]
    public float bloomFactor = 0.5f;

    /// <summary>
    /// Bloom材质球
    /// </summary>
    public Material _Material;

    /// <summary>
    /// 特定渲染图
    /// </summary>
    public RenderTexture m_R;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material)
        {
            RenderTexture temp1 = RenderTexture.GetTemporary(m_R.width, m_R.height, 0, m_R.format);
            RenderTexture temp2 = RenderTexture.GetTemporary(m_R.width, m_R.height, 0, m_R.format);

            //复制泛光图
            Graphics.Blit(m_R, temp1);


            //根据阈值提取高亮部分,使用pass0进行高亮提取
            _Material.SetVector("_colorThreshold", colorThreshold);
            Graphics.Blit(temp1, temp2, _Material, 0);

            //高斯模糊，两次模糊，横向纵向，使用pass1进行高斯模糊
            _Material.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
            Graphics.Blit(temp2, temp1, _Material, 1);
            _Material.SetVector("_offsets", new Vector4(samplerScale, 0, 0, 0));
            Graphics.Blit(temp1, temp2, _Material, 1);

            //Bloom，将模糊后的图作为Material的Blur图参数
            _Material.SetTexture("_BlurTex", temp2);
            _Material.SetVector("_bloomColor", bloomColor);
            _Material.SetFloat("_bloomFactor", bloomFactor);

            //使用pass2进行景深效果计算，清晰场景图直接从source输入到shader的_MainTex中
            Graphics.Blit(source, destination, _Material, 2);

            //释放申请的RT
            RenderTexture.ReleaseTemporary(temp1);
            RenderTexture.ReleaseTemporary(temp2);
        }
    }
}


