using System.Collections.Generic;
using RAIN.Core;
using RAIN.Entities.Aspects;
using RAIN.Serialization;
using RAIN.Perception.Sensors;
using RAIN.Perception.Sensors.Filters;


[RAINSerializableClass, RAINElement("Tag Filter")]
public class PriorityFilter : RAINSensorFilter {
    [RAINSerializableField(Visibility = FieldVisibility.Show, ToolTip = "The tags to detect")]
    public List<string> visibleTags = new List<string>();

    private List<string> tagsInSight = new List<string>();

    public override void Filter(RAINSensor aSensor, List<RAINAspect> aValues) {
        for (int i = 0; i < aValues.Count; ++i) {
            if (!visibleTags.Contains(aValues[i].Entity.Form.tag))
                aValues.RemoveAt(i);
            else
                tagsInSight.Add(aValues[i].Entity.Form.tag);
        }

        for (int i = 0; i < visibleTags.Count; ++i) {
            if (tagsInSight.Contains(visibleTags[i])) {
                for (int j = 0; j < aValues.Count; ++i) {
                    if (aValues[i].Entity.Form.tag != visibleTags[i]) {
                        aValues.RemoveAt(i);
                        break;
                    }
                }
            }
        }

    }
}
