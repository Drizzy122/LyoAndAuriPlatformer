// external functions
EXTERNAL StartQuest(questId)
EXTERNAL AdvanceQuest(questId)
EXTERNAL FinishQuest(questId)

// quest ids (questId + "Id for variable name)
VAR CollectEluminsQuestId = "CollectEluminsQuest"

// quest states (questId + "State" for variable name
VAR CollectEluminsQuestState = "REQUIREMENTS_NOT_MET"

// ink files
INCLUDE collect_elumins_start_npc.ink
//INCLUDE collect_elumins_finish_npc.ink