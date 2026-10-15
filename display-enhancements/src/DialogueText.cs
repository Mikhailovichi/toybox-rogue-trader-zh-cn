using System;
using System.Collections.Generic;
using System.Linq;

namespace ToyBoxDisplayEnhancements {
    internal static class DialogueText {
        private const string Answer = "Kingmaker.DialogSystem.Blueprints.BlueprintAnswer";
        private const string AnswersList = "Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList";
        private const string Cue = "Kingmaker.DialogSystem.Blueprints.BlueprintCue";
        private const string Check = "Kingmaker.DialogSystem.Blueprints.BlueprintCheck";
        private const string Dialog = "Kingmaker.DialogSystem.Blueprints.BlueprintDialog";
        private const string BookPage = "Kingmaker.DialogSystem.Blueprints.BlueprintBookPage";
        private const string Sequence = "Kingmaker.DialogSystem.Blueprints.BlueprintCueSequence";
        private const string Exit = "Kingmaker.DialogSystem.Blueprints.BlueprintSequenceExit";

        internal static bool Supported(object blueprint) {
            switch (blueprint?.GetType().FullName) {
                case Answer: case AnswersList: case Cue: case Check:
                case Dialog: case BookPage: case Sequence: case Exit: return true;
                default: return false;
            }
        }

        private sealed class Sample {
            internal string Text;
            internal bool IsAnswer;
        }

        internal static string FullText(object blueprint) => ReadText(blueprint, new HashSet<object>())?.Text;

        private static Sample ReadText(object blueprint, HashSet<object> visited) {
            if (blueprint == null || !visited.Add(blueprint)) return null;
            var type = blueprint.GetType().FullName;
            if (type == Answer || type == Cue) {
                var text = Reflect.Localized(Reflect.Read(blueprint, "Text"));
                if (Text.Missing(text)) return null;
                text = Text.Clean(text);
                return Text.Missing(text) ? null : new Sample { Text = text, IsAnswer = type == Answer };
            }
            // Only follow declared collections. Never select a cue or inspect check outcomes.
            if (type == AnswersList) return ReadItems(Reflect.Read(blueprint, "Answers"), visited);
            if (type == Dialog) return ReadItems(Reflect.Read(Reflect.Read(blueprint, "FirstCue"), "Cues"), visited);
            if (type == BookPage)
                return ReadItems(Reflect.Read(blueprint, "Cues"), visited) ?? ReadItems(Reflect.Read(blueprint, "Answers"), visited);
            if (type == Sequence)
                return ReadItems(Reflect.Read(blueprint, "Cues"), visited) ?? ReadText(Reflect.Resolve(Reflect.Read(blueprint, "Exit")), visited);
            if (type == Exit)
                return ReadItems(Reflect.Read(Reflect.Read(blueprint, "Continue"), "Cues"), visited) ?? ReadItems(Reflect.Read(blueprint, "Answers"), visited);
            return null;
        }

        private static Sample ReadItems(object items, HashSet<object> visited) {
            foreach (var reference in Reflect.Items(items)) {
                var sample = ReadText(Reflect.Resolve(reference), visited);
                if (sample != null) return sample;
            }
            return null;
        }

        internal static string Identity(object blueprint) {
            var type = blueprint?.GetType().FullName;
            if (type == Check) return CheckName(blueprint) ?? Reflect.BlueprintName(blueprint);
            if (type == Dialog || type == BookPage || type == Sequence || type == Exit)
                return Name(blueprint) ?? Reflect.BlueprintName(blueprint);
            var excerpt = Text.Excerpt(FullText(blueprint));
            if (!Text.Missing(excerpt)) return type == AnswersList
                ? UiText.Get("AnswersListReference", excerpt) : excerpt;
            return Reflect.BlueprintName(blueprint);
        }

        internal static string CheckName(object blueprint) {
            var type = Reflect.String(blueprint, "Type");
            if (Text.Missing(type)) return null;
            var difficulty = Reflect.String(blueprint, "Difficulty");
            string label;
            switch (difficulty) {
                case "Custom": label = UiText.Get("BaseDC", Reflect.Int(blueprint, "DC")); break;
                case "Easy": case "Normal": case "Hard": case "Impossible": label = UiText.Get("Difficulty" + difficulty); break;
                default: return null;
            }
            var result = UiText.Get("ConfiguredCheck", Text.Clean(Runtime.Localize(type)), label);
            if (Reflect.Bool(blueprint, "Hidden")) result = UiText.Get("CheckHidden", result);
            if (Reflect.Items(Reflect.Read(blueprint, "DCModifiers")).Any()) result = UiText.Get("CheckModifiers", result);
            return result;
        }

        internal static string Name(object blueprint) {
            if (!Supported(blueprint) || Aliases.OfficialName(blueprint) != null) return null;
            var type = blueprint.GetType().FullName;
            if (type == Check) return CheckName(blueprint);
            var sample = ReadText(blueprint, new HashSet<object>());
            var excerpt = Text.Excerpt(sample?.Text);
            if (Text.Missing(excerpt)) return null;
            if (type == Answer || type == AnswersList || type == Cue)
                return UiText.Get(type == Answer ? "AnswerTitle" : type == AnswersList ? "AnswersListTitle" : "CueTitle", excerpt);
            var label = type == Dialog ? "DialogLabel" : type == BookPage ? "BookPageLabel" : type == Sequence ? "SequenceLabel" : "ExitLabel";
            return UiText.Get("ContainerTitle", UiText.Get(label), UiText.Get(sample.IsAnswer ? "AnswersListReference" : "CueReference", excerpt));
        }

        internal static string Title(object blueprint, Func<string, string> formatter) {
            var name = Name(blueprint);
            if (name == null) return null;
            var title = Text.Safe(name);
            if (formatter != null) title = formatter(title);
            var internalName = Reflect.InternalName(blueprint);
            if (Runtime.Setting("ToggleBPsShowDisplayAndInternalName") && !Text.Missing(internalName))
                title += " : <color=#808080>" + Text.Safe(internalName) + "</color>";
            return title;
        }

        internal static string DescriptionFor(object blueprint, string original) {
            if (!Supported(blueprint)) return original;
            if (Text.Missing(Text.Clean(original))) {
                var official = Reflect.Localized(Reflect.Read(blueprint, "Description"));
                if (!Text.Missing(Text.Clean(official))) original = official;
            }
            var type = blueprint.GetType().FullName;
            var sample = ReadText(blueprint, new HashSet<object>());
            var text = type == Check ? CheckName(blueprint) : sample?.Text;
            if (Text.Missing(text) || Text.Clean(original) == text) return original;
            var key = type == Check ? "CheckDescription" : type == Cue ? "CueText" : type == Answer ? "AnswerText" :
                sample.IsAnswer ? "AnswersListText" : "ContainsCueText";
            var description = UiText.Get(key, Text.Safe(text));
            return Text.Missing(Text.Clean(original)) ? description : original + "\n" + description;
        }

        internal static string SearchText(object blueprint) {
            if (!Supported(blueprint)) return null;
            var name = (Name(blueprint) ?? "") + " " + Text.Excerpt(FullText(blueprint));
            return Runtime.Setting("ToggleSearchDescriptions") ? name + " " + Text.Clean(DescriptionFor(blueprint, null)) : name;
        }
    }
}
