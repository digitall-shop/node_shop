import {useEffect, useState} from 'react';
import {getAdminNodes} from '../../api/nodes/node';
import type {NodeDto} from '../../models/nodes/node';

export default function AdminNodeHealth(){
  const [nodes,setNodes]=useState<NodeDto[]>([]);
  const [loading,setLoading]=useState(true);
  const [error,setError]=useState<string|null>(null);

  useEffect(()=>{(async()=>{
    try{setLoading(true);setNodes(await getAdminNodes());}
    catch(e){setError('Failed to load');}
    finally{setLoading(false);} })();},[]);

  return (
    <div className="p-6">
      <h1 className="text-2xl font-semibold mb-4">Node Agents</h1>
      {loading && <div>Loading...</div>}
      {error && <div className="text-red-400 text-sm">{error}</div>}
      {!loading && !error && (
        <div className="overflow-x-auto border border-gray-700 rounded">
          <table className="min-w-full text-sm">
            <thead className="bg-gray-800">
              <tr>
                <th className="px-3 py-2 text-left">ID</th>
                <th className="px-3 py-2 text-left">Name</th>
                <th className="px-3 py-2 text-left">Host</th>
                <th className="px-3 py-2 text-left">Status</th>
                <th className="px-3 py-2 text-left">Provisioning</th>
                <th className="px-3 py-2 text-left">Agent</th>
                <th className="px-3 py-2 text-left">Target</th>
                <th className="px-3 py-2 text-left">Last Seen (UTC)</th>
                <th className="px-3 py-2 text-left">Install</th>
                <th className="px-3 py-2 text-left">Message</th>
              </tr>
            </thead>
            <tbody>
              {nodes.map(n=>{
                const stale = n.lastSeenUtc ? (Date.now()-Date.parse(n.lastSeenUtc))>5*60*1000 : true;
                return (
                  <tr key={n.id} className="border-t border-gray-700 hover:bg-gray-800/50">
                    <td className="px-3 py-2">{n.id}</td>
                    <td className="px-3 py-2">{n.nodeName}</td>
                    <td className="px-3 py-2">{n.sshHost}:{n.sshPort}</td>
                    <td className="px-3 py-2">{n.status}</td>
                    <td className="px-3 py-2">{n.provisioningStatus}</td>
                    <td className="px-3 py-2">{n.agentVersion || '-'}</td>
                    <td className="px-3 py-2">{n.targetAgentVersion || '-'}</td>
                    <td className={`px-3 py-2 ${stale?'text-yellow-400':''}`}>{n.lastSeenUtc? new Date(n.lastSeenUtc).toISOString().replace('T',' ').slice(0,19):'-'}</td>
                    <td className="px-3 py-2">{n.installMethod}</td>
                    <td className="px-3 py-2 max-w-xs truncate" title={n.provisioningMessage||''}>{n.provisioningMessage||'-'}</td>
                  </tr>
                );
              })}
              {nodes.length===0 && !loading && (
                <tr><td colSpan={10} className="px-3 py-6 text-center text-gray-400">No nodes</td></tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

