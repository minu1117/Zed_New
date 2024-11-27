using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SkillIndicator : MonoBehaviour
{
    public enum AddPivot
    {
        None,

        X,
        Y,
    }

    [SerializeField] private DecalProjector maxSizeProjector;
    [SerializeField] private DecalProjector sizeUpProjector;

    public float minWidth;
    public float maxWidth;

    public float minHeight;
    public float maxHeight;

    [SerializeField] private float addMaxProjectorSizeX;
    [SerializeField] private float addMaxProjectorSizeY;

    private float currentTime = 0f;

    private Vector3 addedSize = new Vector3(0,0,1);

    public float duration { get; set; }
    public bool isUsed { get; set; } = false;

    public bool isPivotAdded = false;
    public AddPivot addPivot;
    private Vector3 defaultPivot;
    private Vector3 currentPivot;

    private GameObject followTarget;

    private void Awake()
    {
        if (sizeUpProjector != null)
        {
            sizeUpProjector.size = new Vector3(minWidth, minHeight, 1);
            sizeUpProjector.gameObject.SetActive(false);
        }

        if (maxSizeProjector != null)
        {
            maxSizeProjector.size = new Vector3(maxWidth + addMaxProjectorSizeX, maxHeight + addMaxProjectorSizeY, 1);

            float x = maxSizeProjector.pivot.x;
            float y = maxSizeProjector.pivot.y;
            float z = maxSizeProjector.pivot.z;
            
            switch (addPivot)
            {
                case AddPivot.None:
                    break;
                case AddPivot.X:
                    maxSizeProjector.pivot = new Vector3(maxWidth / 2, y, z);
                    break;
                case AddPivot.Y:
                    maxSizeProjector.pivot = new Vector3(x, maxHeight / 2, z);
                    break;
            }

            maxSizeProjector.gameObject.SetActive(false);
        }

        defaultPivot = sizeUpProjector.pivot;
        currentPivot = sizeUpProjector.pivot;
    }

    private void Update()
    {
        SizeUp();
        
        if (followTarget != null)
        {
            transform.position = followTarget.transform.position;
        }
    }

    private void SizeUp()
    {
        if (!isUsed)
            return;

        if (currentTime > duration)
        {
            ResetProjector();
            return;
        }

        currentTime += Time.deltaTime;
        addedSize.x = Mathf.Lerp(minWidth, maxWidth, currentTime / duration);
        addedSize.y = Mathf.Lerp(minHeight, maxHeight, currentTime / duration);
        addedSize.z = 1f;

        sizeUpProjector.size = addedSize;
        SizeUpPivot(sizeUpProjector);
    }

    private void SizeUpPivot(DecalProjector projector)
    {
        if (!isPivotAdded)
            return;

        switch (addPivot)
        {
            case AddPivot.None:
                break;
            case AddPivot.X:
                currentPivot.x = Mathf.Lerp(defaultPivot.x, maxSizeProjector.pivot.x, currentTime / duration);
                break;
            case AddPivot.Y:
                currentPivot.y = Mathf.Lerp(defaultPivot.y, maxSizeProjector.pivot.y, currentTime / duration);
                break;
        }

        projector.pivot = currentPivot;
    }

    public void Use()
    {
        sizeUpProjector.gameObject.SetActive(true);
        maxSizeProjector.gameObject.SetActive(true);
        currentTime = 0f;
        isUsed = true;
    }

    public void ResetProjector()
    {
        sizeUpProjector.gameObject.SetActive(false);
        maxSizeProjector.gameObject.SetActive(false);

        isUsed = false;
        currentTime = 0f;
        addedSize = Vector2.zero;
        sizeUpProjector.size = new Vector2(minWidth, minHeight);
    }

    public void SetTarget(GameObject followTarget)
    {
        this.followTarget = followTarget;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
