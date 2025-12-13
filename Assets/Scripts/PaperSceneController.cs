using UnityEngine;
using System.Collections.Generic;

public class PaperSceneController : MonoBehaviour
{
    public List<GameObject> papers;  


    private MaterialPropertyBlock block;
    private int localIndex = 0;

    private void Start()
    {
        block = new MaterialPropertyBlock();

        int globalIndex = PaperSequenceManager.Instance != null
            ? PaperSequenceManager.Instance.CurrentIndex
            : 0;

        if (globalIndex >= 0)
            localIndex = globalIndex;
        else
            localIndex = 0;
        
        EnablePreviousPaperTexts();

        HighlightCurrent();
    }
    
    void EnablePreviousPaperTexts()
    {
        for (int i = 0; i < localIndex; i++)
        {
            if (i < papers.Count)
            {
                var paper = papers[i];

                var textObj = paper.transform.Find("Text");

                if (textObj != null)
                    textObj.gameObject.SetActive(true);
            }
        }
    }


    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            OnTriggerPressed();
        }
    }

    private void OnTriggerPressed()
    {
        if (localIndex < 0 || localIndex >= papers.Count)
            return;

        var currentPaper = papers[localIndex];

        var paperItem = currentPaper.GetComponent<PaperItem>();
        if (paperItem != null)
            paperItem.InvokeTriggered();
        else
            Debug.LogWarning("Paper 没有 PaperItem 脚本：" + currentPaper.name);

        Unhighlight(currentPaper);

        PaperSequenceManager.Instance.AdvanceIndex();

        localIndex++;
        if (localIndex < papers.Count)
        {
            HighlightCurrent();
        }
        else
        {
            Debug.Log("本场景纸全部处理完毕");
        }
    }


    void HighlightCurrent()
    {
        if (localIndex < papers.Count)
            Highlight(papers[localIndex]);
    }

    void Highlight(GameObject obj)
    {
        var rend = obj.GetComponent<Renderer>();
        if (!rend) return;

        rend.GetPropertyBlock(block);
        rend.material.EnableKeyword("_EMISSION");
        rend.SetPropertyBlock(block);
    }

    void Unhighlight(GameObject obj)
    {
        var rend = obj.GetComponent<Renderer>();
        if (!rend) return;

        rend.GetPropertyBlock(block);
        rend.material.DisableKeyword("_EMISSION");
        rend.SetPropertyBlock(block);
    }
}
