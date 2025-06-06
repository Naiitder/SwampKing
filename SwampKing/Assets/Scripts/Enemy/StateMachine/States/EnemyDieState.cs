using System;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

public class EnemyDieState : EnemyBaseState
{
    public EnemyDieState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) {
    }

    public override void EnterState(){
        
        _ctx.Agent.stoppingDistance = 0f;
        _ctx.Agent.SetDestination(_ctx.transform.position);
        _ctx.EnemyAnimatorController.Animator.SetBool(_ctx.EnemyAnimatorController.IsDeadHash, true);
        
        _ctx.StartCoroutine(DropCoinsAfterDelay(1f));
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    { }
    
    public override void InitializeSubState(){
        
    }

    public override void CheckSwitchStates()
    {}
    
    private System.Collections.IEnumerator DropCoinsAfterDelay(float delay)
    {
        int coins = GetCoinsFromDB(_ctx.EnemyManager.CharacterStats.ID);
        
        yield return new WaitForSeconds(delay);
        
        if (coins > 0 && _ctx.CoinPrefab != null)
        {
            GameObject coinObject = Object.Instantiate(_ctx.CoinPrefab,
                _ctx.transform.position+_ctx.transform.up, Quaternion.identity);
            Coins coinScript = coinObject.GetComponent<Coins>();
            if (coinScript != null)
            {
                coinScript.coins = coins; 
            }
        }
        
        yield return new WaitForSeconds(1f);
        RemoveAllScriptsExceptStateMachineAndAnimator();
        
    }
    
    private int GetCoinsFromDB(int characterId)
    {
        int coins = 0;
        using (var connection = new Mono.Data.Sqlite.SqliteConnection(SQLiteDB.instance.dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT coins FROM character WHERE id = @id;";
                command.Parameters.AddWithValue("@id", characterId);

                var result = command.ExecuteScalar();
                if (result != null && result != System.DBNull.Value)
                {
                    coins = Convert.ToInt32(result);
                }
            }
        }

        return coins;
    }
    
    private void RemoveAllScriptsExceptStateMachineAndAnimator()
    {
        MonoBehaviour[] scripts = _ctx.GetComponents<MonoBehaviour>();

        Collider[] colliders = _ctx.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            Object.Destroy(collider);
        }
        
        NavMeshAgent agent = _ctx.GetComponent<NavMeshAgent>();
        Object.Destroy(agent);
        
        Rigidbody rb = _ctx.GetComponent<Rigidbody>();
        Object.Destroy(rb);
        
        foreach (var script in scripts)
        {
            Object.Destroy(script);
        }
        
        Object.Destroy(_ctx.EnemyManager);
        

    }
}