using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PSOStats : MonoBehaviour
{
    public TextMeshProUGUI bestFitText;
    public TextMeshProUGUI avgFitText;

    void Update()
    {
        PSORender psoRender = FindObjectOfType<PSORender>();
        if (psoRender)
        {
            var pso = psoRender.pso;
            if (pso != null)
            {
                bestFitText.text = "Best Fit = " + pso.BestSoFar.fitness;
                avgFitText.text = "Avg Fit = " + pso.AvgFitCurr;
            }
        }
    }
}
