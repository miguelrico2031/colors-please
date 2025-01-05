using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BucketGenerator : MonoBehaviour
{
    [SerializeField] private SwipeController _swipeController;
    [SerializeField] private RectTransform _bucketUIGroup;
    [SerializeField] private BucketUI _bucketUIPrefab;
    [SerializeField] private BucketUI _piggyBankBucketUIPrefab;


    private int _bucketCount;

    private void Start()
    {
        GenerateBuckets();
    }

    private void GenerateBuckets()
    {
        _bucketCount = 0;
        
        List<Bucket> buckets = new List<Bucket>();
        var day = ServiceLocator.Get<IDayService>().CurrentDay;
        buckets.AddRange(day.Buckets);
        
        
        foreach (var bucket in buckets)
            CreateBucketUI(bucket, _bucketUIPrefab);
        
        CreateBucketUI(null, _piggyBankBucketUIPrefab);

        _bucketUIGroup.position -= (_bucketCount - 1) * (_swipeController.ItemStep / 2f);
        _swipeController.Initialize(_bucketCount);
    }


    private void CreateBucketUI(Bucket bucket, BucketUI prefab)
    {
        var bucketUI = Instantiate(prefab, _bucketUIGroup);
        bucketUI.Initialize(bucket);
        _bucketCount++;
    }
}