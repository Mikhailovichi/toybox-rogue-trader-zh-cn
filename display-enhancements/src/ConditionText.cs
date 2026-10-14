using System.Collections.Generic;
using System.Linq;

namespace ToyBoxDisplayEnhancements {
    internal sealed class ConditionText {
        internal string Summary;
        internal string NegativeSummary;
        internal string Operation;
        internal List<ConditionText> Children;
        internal bool Not;
        internal string Details;

        internal string Render() {
            string result;
            if (Children == null) result = Not ? NegativeSummary ?? UiText.Get("NotSatisfied", Summary) : Summary;
            else if (Children.Count == 0) result = "";
            else if (Children.Count == 1 && !Not) result = Children[0].Render();
            else {
                var heading = UiText.Get(Operation == "And" ? "AllConditions" : "AnyConditions");
                var body = string.Join("\n", Children.Select((c, i) => "  " + (i + 1) + ". " + c.Render().Replace("\n", "\n     ")));
                result = UiText.Get("Group", heading, body);
                if (Not) result = UiText.Get("NotGroup", result.Replace("\n", "\n  "));
            }
            return Text.WithDetails(result, Details);
        }
    }
}
