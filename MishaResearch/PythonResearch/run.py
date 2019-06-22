import numpy as np
import pandas as pd

import markov_clustering as mc
import networkx as nx

nodes = pd.read_json(r"input.graph", orient='records', lines = True)

nodes = nodes.sort_values('Id')
nodes['cluster_0'] = nodes.Id
edges = pd.DataFrame(np.repeat(nodes.Id, nodes.apply(lambda x: len(x.ConnectedIds), axis = 1)))
edges['connectedId'] = np.concatenate(np.array(nodes.ConnectedIds))

clusters_count = 6
j=1
while clusters_count > 5:
    old_cluster_field = f"cluster_{j-1}"
    new_cluster_field = f"cluster_{j}"

    id_nodes = nodes.set_index('Id')
    edges['cluster1'] = np.array(id_nodes.loc[edges.Id][old_cluster_field])
    edges['cluster2'] = np.array(id_nodes.loc[edges.connectedId][old_cluster_field])

    edges_strength=edges.groupby(['cluster1', 'cluster2']).size().reset_index()
    edges_strength=edges_strength[edges_strength.cluster1 != edges_strength.cluster2]

    graph = nx.Graph()
    graph.add_weighted_edges_from(edges_strength.apply(lambda x: (x.cluster1, x.cluster2,1), axis = 1))

    matrix = nx.adjacency_matrix(graph, nodelist=np.arange(0, nodes[old_cluster_field].max() + 1))
    result = mc.run_mcl(matrix, inflation=2, expansion=2, iterations=100)
    clusters = mc.get_clusters(result)
    clusters_count = (len(clusters))
    print("\t"+str(clusters_count))
    nodes_clusters = np.zeros(nodes[old_cluster_field].max() + 1, dtype=int)

    for i in range(len(clusters)):
        nodes_clusters[list(clusters[i])] = i
        nodes[new_cluster_field] = nodes[old_cluster_field].map(lambda x: nodes_clusters[x])
        nodes[new_cluster_field+'_color'] = nodes[new_cluster_field].map(lambda i: cluster_colors[i])

        j += 1

        clusters_columns = [col for col in nodes.filter(like = 'cluster_').columns if not 'color' in col][1:]
        nodes['cluster_hierarchy'] = nodes.apply(lambda x: [x[c] for c in clusters_columns],axis = 1)
        nodes[['Id', 'X', 'Y', 'cluster_hierarchy', 'ConnectedIds']].to_json(r"result.clusters", orient='records', lines = True)