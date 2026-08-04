using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

[CreateAssetMenu(
    fileName = "XXX-RitualSpell-Name",
    menuName = "Cards/RitualSpell",
    order = 5
)]
public class RitualSpellCard : SpellCard
{
    private const int MATERIAL_COUNT = 3; // apparently every ritual in YGOFM has exactly 3 materials

    public override SpellCardType GetSpellCardType() => SpellCardType.Ritual;
    public List<MonsterCard> cardMaterialList = new(MATERIAL_COUNT); 
    public RitualMonsterCard cardResult;


    // More context can be read at NormalSpellCard's Activate()
    // NOTE: if multiple material exist, the first one will always be chosen
    // exception is if the materials has dupe (like Blue Eyes Ultimate Dragon), then previous sentence doesnt really matter
    public override bool Activate()
    {
        Debug.Assert(cardResult != null);
        Debug.Assert(cardMaterialList != null);
        Debug.Assert(cardMaterialList.Count == MATERIAL_COUNT);


        List<Card> cardMaterialListCopy = new(cardMaterialList);
        List<FieldCard> materialFieldCardList = new();
        FieldSystem callerFieldSystem = GameplayManager.Instance().FieldSystem();
        List<FieldCardContainer> fieldCardContainers = callerFieldSystem.GetFrontRankContainers();

        bool ritualSucceed = false; // default
        for(int i=0; i < fieldCardContainers.Count; i++) 
        {
            var fieldCardContainer = fieldCardContainers[i];
            if(fieldCardContainer.IsEmpty()) continue;
            
            var fieldCard = fieldCardContainer.GetCard();
            var card = fieldCard.GetCardData();
            if(cardMaterialListCopy.Exists(e => e == card) == false) continue;

            // valid
            cardMaterialListCopy.Remove(card);
            materialFieldCardList.Add(fieldCard);


            if(cardMaterialListCopy.Count == 0){
                ritualSucceed = true;
                break; 
            }
        }

        if (ritualSucceed) // TODO: proper activation with ritual animation
        {
            Debug.Assert(materialFieldCardList.Count > 0);

            // get target container for placing ritual monster result later
            FieldCardContainer targetFieldCardContainer = materialFieldCardList[0].GetContainer();

            // destroys all material
            materialFieldCardList.ForEach(e => e.Destroy());

            // spawn ritual monster on the target container
            callerFieldSystem.SpawnFieldCard(
                cardResult, 
                false, // always face up
                GuardianStar.NONE, // dummy, TODO: properly handle guardian star via fusion system's resolve panel
                null, // null modifier list, the default value will be hanled in the function logic
                targetFieldCardContainer: targetFieldCardContainer
            );

            //// next: handle ritual monster's guardian star
        }

        Debug.Log($"Activated RITUAL SPELL! ritualSucceed:{ritualSucceed}");
        return ritualSucceed;
    }
}
