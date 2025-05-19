import re
import sys

def algebraic_to_index(sqn):
    files = "abcdefgh"
    ranks = "12345678"

    if len(sqn) != 2 or sqn[0] not in files or sqn[1] not in ranks:
        raise ValueError(f"Invalid square notation: {sqn}")

    file = files.index(sqn[0])
    rank = int(sqn[1]) - 1  # 1-based to 0-based
    return rank * 8 + file



def parse_log(file_path):
    with open(file_path, 'rb') as f:
        text = f.read().decode('utf-8', errors='ignore')

    results = {
        "rooks": {},
        "bishops": {}
    }

    current_piece = None
    lines = text.splitlines()

    for line in lines:
        line = line.strip()

        if "Initializing Rook attack table" in line:
            current_piece = "rooks"
        elif "Initializing Bishop attack table" in line:
            current_piece = "bishops"
        elif "attack table initialized" in line:
            current_piece = None
        elif current_piece:
            blocker_match = re.match(r"Blocker (\d+) at square (\d+): ([0-9A-Fa-f]{16})", line)
            attack_match = re.match(r"Attack perm (\d+) at square (\d+) placed in index (\d+): ([0-9A-Fa-f]{16})", line)

            if blocker_match:
                permidx, sq, value = blocker_match.groups()
                permidx, sq = int(permidx), int(sq)
                results[current_piece].setdefault(sq, {})[permidx] = {
                    "blocker": value
                }

            elif attack_match:
                permidx, sq, idx, value = attack_match.groups()
                permidx, sq, idx = int(permidx), int(sq), int(idx)
                if sq in results[current_piece] and permidx in results[current_piece][sq]:
                    results[current_piece][sq][permidx]["attack"] = value
                    results[current_piece][sq][permidx]["index"] = idx
                else:
                    print(f"Warning: No blocker found for square {sq} and permidx {permidx}. Creating entry.")
                    results[current_piece].setdefault(sq, {})[permidx] = {
                        "attack": value,
                        "index": idx
                    }

                if "by_index" not in results[current_piece].setdefault(sq, {}):
                    results[current_piece][sq]["by_index"] = {}
                results[current_piece][sq]["by_index"][idx] = permidx

    return results

def print_bitboard(bitboard):
    """Prints a 64-bit bitboard (int or hex str) in standard chessboard layout (A1=0 to H8=63)."""
    if isinstance(bitboard, str):
        bitboard = int(bitboard, 16)

    for rank in reversed(range(8)):
        print(f"{rank + 1} ", end="")
        for file in range(8):
            square = rank * 8 + file
            mask = 1 << square
            print("1 " if bitboard & mask else ". ", end="")
        print()
    print("  a b c d e f g h\n")


def interactive_browser(data):
    piece_type = "rooks"
    square = 0
    perm = 0
    index = 0
    use_index_mode = False  # False means perm mode, True means index mode

    print("\n=== Sliding Piece Log Browser ===")
    print("Commands:")
    print("  piece rook/bishop | sq [0-63] | sqn[a1-h8] | index [0+] | perm [0+] | mode perm/index | next | prev | show | exit")

    while True:
        cmd = input(">>> ").strip().lower()

        if cmd in ["exit", "quit", "q"]:
            break

        elif cmd.startswith("piece"):
            if "bishop" in cmd:
                piece_type = "bishops"
            elif "rook" in cmd:
                piece_type = "rooks"
            else:
                print("Invalid piece type. Use 'rook' or 'bishop'.")

        elif cmd.startswith("sq "):
            try:
                square = int(cmd.split()[1])
            except ValueError:
                print("Invalid square index. Use 0–63.")

        elif cmd.startswith("sqn "):
            try:
                sqn_str = cmd.split()[1].lower()
                square = algebraic_to_index(sqn_str)
            except Exception as e:
                print(e)

        elif cmd.startswith("perm "):
            try:
                perm = int(cmd.split()[1])
                use_index_mode = False
                # Try to sync index from perm
                if square in data[piece_type] and perm in data[piece_type][square]:
                    index = data[piece_type][square][perm].get("index", index)
            except Exception:
                print("Invalid permutation.")

        elif cmd.startswith("index "):
            try:
                index = int(cmd.split()[1])
                use_index_mode = True
                # Try to sync perm from index
                if square in data[piece_type] and "by_index" in data[piece_type][square]:
                    perm = data[piece_type][square]["by_index"].get(index, perm)
            except Exception:
                print("Invalid index.")

        elif cmd.startswith("mode "):
            mode = cmd.split()[1]
            if mode in ["perm", "index"]:
                use_index_mode = (mode == "index")
                print(f"Switched mode to {mode}")
            else:
                print("Unknown mode. Use 'perm' or 'index'.")

        elif cmd == "next":
            if use_index_mode:
                index += 1
                # Sync perm if possible
                if square in data[piece_type] and "by_index" in data[piece_type][square]:
                    perm = data[piece_type][square]["by_index"].get(index, perm)
            else:
                perm += 1
                # Sync index if possible
                if square in data[piece_type] and perm in data[piece_type][square]:
                    index = data[piece_type][square][perm].get("index", index)

        elif cmd == "prev":
            if use_index_mode:
                index = max(0, index - 1)
                # Sync perm if possible
                if square in data[piece_type] and "by_index" in data[piece_type][square]:
                    perm = data[piece_type][square]["by_index"].get(index, perm)
            else:
                perm = max(0, perm - 1)
                # Sync index if possible
                if square in data[piece_type] and perm in data[piece_type][square]:
                    index = data[piece_type][square][perm].get("index", index)

        elif cmd == "show":
            try:
                if square not in data[piece_type]:
                    print(f"No data for square {square}")
                    continue

                if use_index_mode:
                    # Lookup perm via index
                    by_index = data[piece_type][square].get("by_index", {})
                    perm = by_index.get(index)
                    if perm is None:
                        print(f"No data for index {index} on square {square}")
                        continue
                # Now perm is set, get entry
                entry = data[piece_type][square].get(perm)
                if entry is None:
                    print(f"No data for perm {perm} on square {square}")
                    continue

                print(f"\n{piece_type.capitalize()} - Square {square}, Perm {perm}, Index {entry.get('index', 'N/A')}")
                print("Blocker:")
                print_bitboard(entry['blocker'])
                print(f"Attack at index {entry.get('index', 'N/A')}:")
                print_bitboard(entry['attack'])

            except KeyError:
                print("No data for that square/index.")
        else:
            print("Unknown command.")


# Run the program
if __name__ == "__main__":
    log = sys.argv[1:]
    if not log:
        log = parse_log("logs/log.bin")
    interactive_browser(log)
