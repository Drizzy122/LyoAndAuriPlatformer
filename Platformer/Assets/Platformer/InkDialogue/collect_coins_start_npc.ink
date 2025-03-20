=== collectCoinsStart ===
{ CollectCoinsQuestState :
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": ->  finished
    - else: -> END
}
            
= requirementsNotMet
// not possible for this quest, but putting something here anyways
Come back once you've leveled up a bit more.
-> END

= canStart
Will you collect 5 Elumin Orbs and bring them to my friend over there ?
*[Yes]
    ~StartQuest("CollectCoinsQuest")
    Great!
*[No]
    oh, ok then. Come Back if you change your mind.
- -> END

= inProgress
How is the hunt for those Elumin Orbs going?
-> END

= canFinish
oh? You collected the Elumin Orbs? Go give them to my friend over there and he'll give you a reward!
-> END

= finished
Thanks for collecting those Elumin Orbs!
-> END