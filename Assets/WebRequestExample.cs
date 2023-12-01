using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestExample : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(GetPirStatus());
    }

    IEnumerator GetPirStatus()
    {
        string url = "http://192.168.3.26//getPirStatus";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                // Parse the response to extract the PirStatus
                string responseText = www.downloadHandler.text;
                int pirStatus = ParsePirStatus(responseText);
                Debug.Log("PirStatus: " + pirStatus);
            }
        }
    }

    int ParsePirStatus(string responseText)
    {
        // Extract the PirStatus from the responseText
        // Assuming the response is in the format "PirStatus: X"
        string[] parts = responseText.Split(':');
        if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out int pirStatus))
        {
            return pirStatus;
        }

        // Return a default value or handle parsing error
        return -1;
    }
}
