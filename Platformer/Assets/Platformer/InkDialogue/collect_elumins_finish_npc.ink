=== collectEluminsFinish ===
{ CollectEluminsQuestState:
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

* { CollectEluminsQuestState == "CAN_FINISH"} [here are some coins.]
    ~ FinishQuest(CollectEluminsQuestId)
    Oh? these Elumin Orbs are for me? thank you!
-> END
