using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupPulic : ToggleGroup
{
    // Start is called before the first frame update
    
   

        public List<Toggle> GetToggles()
        {
            return m_Toggles;
        }

    
}
