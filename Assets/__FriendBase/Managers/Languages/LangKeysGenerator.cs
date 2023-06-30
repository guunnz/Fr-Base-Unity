using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

class Item
{
    [JsonProperty("@key")]
    public string Key { get; set; }

    [JsonProperty("@value")]
    public string Value { get; set; }
}

class Lang
{
    public List<Item> item { get; set; }
}

class Root
{
    [JsonProperty("?xml")]
    public Xml Xml { get; set; }
    public Texts texts { get; set; }
}

class Texts
{
    public Lang lang { get; set; }
}

class Xml
{
    [JsonProperty("@version")]
    public string Version { get; set; }

    [JsonProperty("@encoding")]
    public string Encoding { get; set; }
}

class LangKeysGenerator : MonoBehaviour
{
    void Start()
    {
        XmlDocument doc = new XmlDocument();
        Debug.Log(Application.dataPath + "/Resources/language/lang_en.xml");
        doc.Load(Application.dataPath + "/Resources/language/lang_en.xml");
        string jsonText = JsonConvert.SerializeXmlNode(doc);

        Root ROOT = JsonConvert.DeserializeObject<Root>(jsonText);

        List<Item> langItems = ROOT.texts.lang.item;

        string KeysClass = "";



        KeysClass += "public class LangKeys " + Environment.NewLine;
        KeysClass += "{" + Environment.NewLine;

        foreach (Item item in langItems)
        {
            if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value) || KeysClass.Contains(item.Key.Replace(".", "_").Replace(" ", "").Replace("!", "").ToUpper().Replace(":", "_")))
            {
                continue;
            }
            
            KeysClass += "public const string " + item.Key.Replace(".", "_").Replace(" ", "").Replace("!", "").Replace(":","_").ToUpper() + " = \"" + item.Key + "\";" + Environment.NewLine;
        }
        KeysClass += "}";
        GUIUtility.systemCopyBuffer = KeysClass;
        Debug.LogError("Success creating lang key class, class is in your clipboard");
    }
}