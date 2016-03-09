using System.Collections.Generic;
using RAIN.Core;
using RAIN.Entities.Aspects;
using RAIN.Serialization;
using RAIN.Perception.Sensors;
using RAIN.Perception.Sensors.Filters;


[RAINSerializableClass, RAINElement("Tag Filter")]
public class StunFilter : RAINSensorFilter {
    [RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The tags to detect")]
    public List<string> visibleTags = new List<string>();

    public override void Filter(RAINSensor aSensor, List<RAINAspect> aValues) {
        for(int i = 0; i < aValues.Count; ++i) {
            if (!visibleTags.Contains(aValues[i].Entity.Form.tag))
                aValues.RemoveAt(i);
        }
    }
}
