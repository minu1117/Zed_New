using UnityEngine;

public enum Skybox
{
    Defalut = 0,
    ShadowOrder,
    BackStreet,
    Kinkou,
    Theater,
}


public class SkyboxChanger : MonoBehaviour
{
    public Material defalutSkybox;
    public Material shadowOrderSkybox;
    public Material backStreetSkybox;
    public Material kinkouSkybox;
    public Material theaterSkybox;

    public void ChangeToDaySkybox(Skybox skybox)
    {
        Material mat = defalutSkybox;

        switch (skybox)
        {
            case Skybox.Defalut:
                mat = defalutSkybox;
                break;
            case Skybox.ShadowOrder:
                mat = shadowOrderSkybox;
                break;
            case Skybox.BackStreet:
                mat = backStreetSkybox;
                break;
            case Skybox.Kinkou:
                mat = kinkouSkybox;
                break;
            case Skybox.Theater:
                mat = theaterSkybox;
                break;
            default:
                break;
        }

        RenderSettings.skybox = mat;
    }

    public void ChangeSunSource(Light light)
    {
        RenderSettings.sun = light;
        DynamicGI.UpdateEnvironment();
    }
}