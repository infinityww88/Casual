﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.EssentialKit;
using VoxelBusters.CoreLibrary;
using Sirenix.OdinInspector;

public class CPIAPManager : MonoBehaviour
{
	private Dictionary<string, IBillingProduct> _products = new	Dictionary<string, IBillingProduct>();
	
    // Start is called before the first frame update
    void Start()
    {
	    Debug.Log($"{BillingServices.IsAvailable()}");
	    BillingServices.InitializeStore();
    }

	private void OnEnable()
	{
		// register for events
		BillingServices.OnInitializeStoreComplete   += OnInitializeStoreComplete;
		BillingServices.OnTransactionStateChange    += OnTransactionStateChange;
		BillingServices.OnRestorePurchasesComplete  += OnRestorePurchasesComplete;
	}
	
	public void Purchase(string id) {
		if (!BillingServices.CanMakePayments()) {
			Debug.LogError($"cannot make payment");
			return;
		}
		if (!_products.ContainsKey(id)) {
			Debug.LogError($"there is no product {id}");
			return;
		}
		
		BillingServices.BuyProduct(_products[id]);
	}

	private void OnDisable()
	{
		// unregister from events
		BillingServices.OnInitializeStoreComplete   -= OnInitializeStoreComplete;
		BillingServices.OnTransactionStateChange    -= OnTransactionStateChange;
		BillingServices.OnRestorePurchasesComplete  -= OnRestorePurchasesComplete;
	}
	
	private void OnInitializeStoreComplete(BillingServicesInitializeStoreResult result, Error error)
	{
		if (error == null)
		{
			// update UI
			// show console messages
			var     products    = result.Products;
			Debug.Log("Store initialized successfully.");
			Debug.Log("Total products fetched: " + products.Length);
			Debug.Log("Below are the available products:");
			for (int iter = 0; iter < products.Length; iter++)
			{
				var     product = products[iter];
				Debug.Log(string.Format("[{0}]: {1}", iter, product));
				_products[product.Id] = product;
			}
		}
		else
		{
			Debug.Log("Store initialization failed with error. Error: " + error);
		}

		var     invalidIds  = result.InvalidProductIds;
		Debug.Log("Total invalid products: " + invalidIds.Length);
		if (invalidIds.Length > 0)
		{
			Debug.Log("Here are the invalid product ids:");
			for (int iter = 0; iter < invalidIds.Length; iter++)
			{
				Debug.Log(string.Format("[{0}]: {1}", iter, invalidIds[iter]));
			}
		}
	}
	
	private void OnTransactionStateChange(BillingServicesTransactionStateChangeResult result)
	{
		var     transactions    = result.Transactions;
		for (int iter = 0; iter < transactions.Length; iter++)
		{
			var     transaction = transactions[iter];
			switch (transaction.TransactionState)
			{
			case BillingTransactionState.Purchased:
				Debug.Log(string.Format("Buy product with id:{0} finished successfully.", transaction.Payment.ProductId));
				break;

			case BillingTransactionState.Failed:
				Debug.Log(string.Format("Buy product with id:{0} failed with error. Error: {1}", transaction.Payment.ProductId, transaction.Error));
				break;
			}
		}
	}
	
	private void OnRestorePurchasesComplete(BillingServicesRestorePurchasesResult result, Error error)
	{
		if (error == null)
		{
			var     transactions    = result.Transactions;
			Debug.Log("Request to restore purchases finished successfully.");
			Debug.Log("Total restored products: " + transactions.Length);
			for (int iter = 0; iter < transactions.Length; iter++)
			{
				var     transaction = transactions[iter];
				Debug.Log(string.Format("[{0}]: {1}", iter, transaction.Payment.ProductId));
			}
		}
		else
		{
			Debug.Log("Request to restore purchases failed with error. Error: " +  error);
		}
	}
}
