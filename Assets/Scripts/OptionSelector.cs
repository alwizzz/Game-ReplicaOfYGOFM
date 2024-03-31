using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public abstract class OptionSelector<T> : MonoBehaviour
{
    [Serializable]
    public class Option
    {
        public T value;
        public Button button;
    }

    [SerializeField] protected Option selectedOption;
    [SerializeField] protected List<Option> options;

    // options and optionButtons are predefined
    public void Setup()
    {
        if (options == null || options.Count == 0)
        {
            print("ERROR: options still null or empty!!!");
            return;
        }

        // Setups button
        for(int i=0; i<options.Count; i++)
        {
            var option = options[i];
            option.button.onClick.RemoveAllListeners();
            option.button.onClick.AddListener(
                () => SelectOption(i)
            );
        }

        SelectOption(0); // initially select the first element by default
    }

    protected void SelectOption(int index)
    {
        if (index < 0 || index >= options.Count)
        {
            print("ERROR: index out of bound, potentially abnormal button, aborting...");
            return;
        }
        if(selectedOption != null)
        {
            selectedOption.button.interactable = true;
        }

        selectedOption = options[index];
        selectedOption.button.interactable = false;
    }

    public T GetSelectedOption() => selectedOption.value;
}
