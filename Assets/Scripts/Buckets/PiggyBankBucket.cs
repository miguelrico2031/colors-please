
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PiggyBankBucket : MonoBehaviour
{
    [SerializeField] private float _sceneEndDelay;
    private BucketUI _bucketUI;
    private BucketUI[] _allBuckets;


    private void Awake()
    {
        _bucketUI = GetComponent<BucketUI>();
        _bucketUI.OnSelect += OnSelect;
        _bucketUI.CostText.text = "";
    }

    private void OnSelect()
    {
        _allBuckets = FindObjectsByType<BucketUI>(FindObjectsSortMode.None);
        ServiceLocator.Get<IMoneyService>().PutInPiggyBank();
        ApplyNotPayedConsequences();
        DisableBuckets();
        StartCoroutine(WaitAndEndScene());
    }
    

    private void ApplyNotPayedConsequences()
    {
        var relationShipService = ServiceLocator.Get<IRelationshipService>();
        var consequences = _allBuckets
            .Where(b => b.CurrentState is not BucketUI.State.Selected)
            .SelectMany(b => b.Bucket.NotPayedConsequences);
        
        foreach (var consequence in consequences)
        {
            relationShipService.RemovePoints(consequence.character, consequence.PointsToRemove);
        }
    }

    private void DisableBuckets()
    {
        foreach(var b in _allBuckets) b.DisableEndScene();
    }

    private IEnumerator WaitAndEndScene()
    {
        yield return new WaitForSeconds(_sceneEndDelay);
        ServiceLocator.Get<IDayService>().EndDay();
    }
}
