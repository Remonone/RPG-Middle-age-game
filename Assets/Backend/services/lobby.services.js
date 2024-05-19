
export const lobbyFilter =
    ({room_name, room_secured, room_level, room_map, room_players, room_host, _id}) =>
        ({room_name, room_secured, room_level, room_map, room_players, room_host, _id});

export const lobbyCredentialsFilter = ({server_ip, server_port}) => ({server_ip, server_port});