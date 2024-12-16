using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class StartField : BoardField
{
    private void Start()
    {
        fname.text = property.fieldname;
    }

    public override void OnPlayerLand(Player pl)
    { }
}
