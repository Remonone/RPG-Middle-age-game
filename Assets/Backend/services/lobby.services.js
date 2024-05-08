
export const lobbyFilter =
    ({room_name, room_secured, room_level, room_map, room_players, room_host, _id}) =>
        ({room_name, room_secured, room_level, room_map, room_players, room_host, _id});

export const lobbyCredentialsFilter = ({ip, port}) => ({ip, port});