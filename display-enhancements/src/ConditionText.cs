using System.Collections.Generic;
using System.Linq;

namespace ToyBoxChineseDisplayEnhancements {
    internal sealed class ConditionText {
        internal string Summary;
        internal string NegativeSummary;
        internal string Operation;
        internal List<ConditionText> Children;
        internal bool Not;
        internal string Details;

        internal string Render() {
            string result;
            if (Children == null) result = Not ? NegativeSummary ?? "不满足（" + Summary + "）" : Summary;
            else if (Children.Count == 0) result = "";
            else if (Children.Count == 1 && !Not) result = Children[0].Render();
            else {
                var heading = Operation == "And" ? "以下条件都成立" : "满足以下任一组";
                var body = string.Join("\n", Children.Select((c, i) => "  " + (i + 1) + ". " + c.Render().Replace("\n", "\n     ")));
                result = heading + "：\n" + body;
                if (Not) result = "以下组合不成立：\n  " + result.Replace("\n", "\n  ");
            }
            return Text.WithDetails(result, Details);
        }
    }
}
