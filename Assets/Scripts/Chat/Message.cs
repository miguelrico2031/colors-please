using System;
using UnityEngine;

[Serializable]
public class Message
{
    public Character Character;
    [TextArea(minLines:4, maxLines:8)]public string Text;
    
    
}   