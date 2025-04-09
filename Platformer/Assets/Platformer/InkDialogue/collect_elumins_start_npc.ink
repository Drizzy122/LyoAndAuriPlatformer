=== collectEluminsStart ===
{ CollectEluminsQuestState :
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": ->  finished
    - else: -> END
}
            
= requirementsNotMet
// not possible for this quest, but putting something here anyways
You’re not quite ready for this task. Come back after you’ve gained a bit more experience.
-> END







= canStart
Hey, traveler! Could I ask a favor?
Can You find an Elumin orb for me please.
* [Sure, I can get one for you.]
    ~ StartQuest(CollectEluminsQuestId)
    Perfect thank you so  much. Bring it back when you find it please.

* [What is this Elumin Orb, exactly?]
    They are these specials orbs that allows us to live.
    
* [Not right now.]
    No worries. Come back if you change your mind.
--> END




= inProgress
How is the hunt for those Elumin Orbs going?
-> END




= canFinish
You found an Elumin Orb? Incredible—thank you!

* [Here you go.]
    ~FinishQuest(CollectEluminsQuestId)
    Its so beautifull isnt it. .

-> END





= finished
Thanks again for your help! That orb made a real difference.
-> END



= default
Hm? What do you want?
*[Nothing, I guess.]
-> END


* { CollectEluminsQuestState == "CAN_FINISH"} [here is the LuminOrb you have requested.]
    ~ FinishQuest(CollectEluminsQuestId)
    Oh? these Elumin Orbs are for me? thank you!
-> END