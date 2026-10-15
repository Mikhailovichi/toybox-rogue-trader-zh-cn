using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.ElementsSystem;

namespace Kingmaker.DialogSystem.Blueprints {
    public class CueSelection {
        public List<Ref> Cues = new List<Ref>();
        public object Select() => throw new InvalidOperationException("Display must not select a cue.");
    }
    public class BlueprintCueBase : Kingmaker.Blueprints.BlueprintScriptableObject {
        public ConditionsChecker Conditions = new ConditionsChecker();
        public bool CanShow() => throw new InvalidOperationException("Display must not check availability.");
    }
    public class BlueprintCheck : BlueprintCueBase {
        public Kingmaker.EntitySystem.Stats.Base.StatType Type = Kingmaker.EntitySystem.Stats.Base.StatType.SkillAwareness;
        public bool Hidden;
        public string Difficulty = "Custom";
        public int DC;
        public DCModifier[] DCModifiers = Array.Empty<DCModifier>();
        public Ref m_Success, m_Fail;
        public object Success => throw new InvalidOperationException("Display must not read check success.");
        public object Fail => throw new InvalidOperationException("Display must not read check failure.");
    }
    public class DCModifier { public int Mod; public ConditionsChecker Conditions = new ConditionsChecker(); }
    public class BlueprintDialog : Kingmaker.Blueprints.BlueprintScriptableObject {
        public CueSelection FirstCue = new CueSelection();
        private string description;
        public string DescriptionEn, DescriptionZh;
        public string Description { get => FixtureText.Resolve(description, DescriptionZh, DescriptionEn); set => description = value; }
        public ConditionsChecker Conditions = new ConditionsChecker();
    }
    public class BlueprintBookPage : BlueprintCueBase {
        public List<Ref> Cues = new List<Ref>(), Answers = new List<Ref>();
    }
    public class BlueprintCueSequence : BlueprintCueBase {
        public List<Ref> Cues = new List<Ref>();
        public Ref m_Exit;
        public object Exit => m_Exit?.Get();
    }
    public class BlueprintSequenceExit : Kingmaker.Blueprints.BlueprintScriptableObject {
        public List<Ref> Answers = new List<Ref>();
        public CueSelection Continue = new CueSelection();
    }
}
namespace Kingmaker.Blueprints {
    public class BlueprintUnit : BlueprintScriptableObject {
        public string NameEn, NameZh;
        public string CharacterName => FixtureText.Resolve(null, NameZh, NameEn);
    }
    public class BlueprintUnlockableFlag : BlueprintScriptableObject {
        public int Value { get => throw new InvalidOperationException("Display must not read flag state."); }
        public bool IsUnlocked => throw new InvalidOperationException("Display must not read flag state.");
    }
}
namespace Kingmaker.Blueprints.Items {
    public class BlueprintItem : Kingmaker.UnitLogic.Progression.Features.BlueprintFeature { }
}
namespace Kingmaker.Blueprints.Items.Components { public class MoneyReplacement { } }
namespace Kingmaker.Designers.EventConditionActionSystem.Evaluators {
    public class DynamicEvaluator {
        public int Evaluations;
        public string Caption = "Dynamic target";
        public virtual string GetCaption() => Caption;
        public object GetValue() { Evaluations++; throw new InvalidOperationException("Display must not evaluate."); }
    }
    public class PlayerCharacter : DynamicEvaluator { }
    public class CompanionInParty : DynamicEvaluator {
        public Ref m_Companion;
        public Kingmaker.Blueprints.BlueprintUnit Companion => (Kingmaker.Blueprints.BlueprintUnit)m_Companion?.Get();
        public int Index;
    }
    public class IntConstant : DynamicEvaluator { public int Value; }
}
namespace Kingmaker.Designers.EventConditionActionSystem.Conditions {
    public class CheckPassed : Condition {
        public Ref m_Check;
        public object CheckBlueprint => m_Check?.Get();
        public override string GetCaption() => (Not ? "Not " : "") + "Check Passed (" + ((Kingmaker.Blueprints.SimpleBlueprint)CheckBlueprint)?.name + ")";
    }
    public class CheckFailed : CheckPassed { public override string GetCaption() => base.GetCaption().Replace("Passed", "Failed"); }
    public class CompanionInParty : Condition {
        public Ref m_companion;
        public object companion => m_companion?.Get();
        public bool MatchWhenActive, MatchWhenRemote, MatchWhenDetached, MatchWhenEx, IncludeDead;
    }
    public class ItemsEnough : Condition {
        public bool Money;
        public Ref m_ItemToCheck;
        public object ItemToCheck => m_ItemToCheck?.Get();
        public int Quantity;
    }
    public class HasFact : Condition {
        public object Unit;
        public Ref m_Fact;
        public object Fact => m_Fact?.Get();
    }
    public class HasBuff : Condition {
        public object Target;
        public Ref m_Buff;
        public object Buff => m_Buff?.Get();
    }
    public class FlagUnlocked : Condition {
        public Ref m_ConditionFlag;
        public object ConditionFlag => m_ConditionFlag?.Get();
        public bool ExceptSpecifiedValues;
        public List<int> SpecifiedValues = new List<int>();
    }
    public class FlagInRange : Condition {
        public Ref m_Flag;
        public object Flag => m_Flag?.Get();
        public int MinValue, MaxValue;
    }
}
namespace Kingmaker.Designers.EventConditionActionSystem.Actions {
    public class MarkAnswersSelected : GameAction {
        public Ref[] m_Answers = Array.Empty<Ref>();
        public IEnumerable<object> Answers => m_Answers.Select(r => r?.Get());
        public override string GetCaption() => "Mark Answers Selected";
    }
    public class UnmarkAnswersSelected : MarkAnswersSelected { public override string GetCaption() => "Unmark Answers Selected"; }
    public class StartDialog : GameAction {
        public Ref m_Dialogue;
        public object Dialogue => m_Dialogue?.Get();
        public object DialogEvaluator, DialogueOwner;
    }
    public class UnlockFlag : GameAction {
        public Ref m_flag;
        public object flag => m_flag?.Get();
        public int flagValue;
    }
    public class LockFlag : GameAction {
        public Ref m_Flag;
        public object Flag => m_Flag?.Get();
    }
    public class IncrementFlagValue : LockFlag {
        public object Value;
        public bool UnlockIfNot;
    }
}
