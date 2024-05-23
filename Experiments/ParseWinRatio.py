import numpy as np

def parse_line(line):
    parts = line.strip().split(' = ')
    wins, loses = map(int, parts[0].split(':'))
    return wins, loses

def process_file(filename):
    with open(filename, 'r') as file:
        win_ratios = []
        minute_counter = 1 
        lines = file.readlines() 
        totalWins = 0
        totalLoses = 0
        for i in range(0, len(lines), 600):
            if i < len(lines):
                loses, wins = parse_line(lines[i])

                wins = wins - totalWins
                loses = loses - totalLoses 
                games = wins + loses

                if games == 0:
                    continue

                totalWins += wins
                totalLoses += loses

                if games > 0:
                    win_ratio = wins / games * 100
                    win_ratios.append((minute_counter, win_ratio))
                else:
                    win_ratios.append((minute_counter, 0)) 
                minute_counter += 1
    
    return np.array(win_ratios)

def main():
    filename = 'winratio.txt' 
    win_ratios = process_file(filename)

    np.savetxt('parsed_wr.txt', win_ratios, fmt='%d %.2f', header='Defender Win Rate Over Time')

if __name__ == '__main__':
    main()
