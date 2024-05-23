import authStore from '@/stores/authStore';
import { HubConnectionBuilder } from '@microsoft/signalr';

const connection = new HubConnectionBuilder()
    .withUrl(`https://localhost:7163/gql/message`)
    .build();

const startConnection = () => {
    if (connection.state === "Disconnected") {
        return connection.start()
            .then(() => {
                const connectionId = connection.connectionId;
                authStore.dispatch("updateConnectionId", connectionId) 
                console.log("last CID",connectionId);
                return connectionId;
            })
            .catch(error => {
                console.error('Error starting SignalR connection:', error);
                throw error; 
            });
    } else {
        console.warn("SignalR connection is not in the 'Disconnected' state.");
        return Promise.resolve(null);
    }
};


export { connection, startConnection };