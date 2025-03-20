=== collectCoinsFinish ===
{ CollectCoinsQuestState:
    - "FINISHED": -> finished
    -else: -> default
}

= finished
Thank you!
-> END

= default
Hm? What do you want?
*[Nothing, I guess.]
-> END

* { CollectCoinsQuestState == "CAN_FINISH"} [here are some coins.]
    ~ FinishQuest(CollectCoinsQuestId)
    Oh? these Elumin Orbs are for me? thank you!
-> END
